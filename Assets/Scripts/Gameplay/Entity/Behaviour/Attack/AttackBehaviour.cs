using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Behaviour.Defence;
using Gameplay.Entity.Behaviour.Move;
using Gameplay.Entity.Stats;
using UniRx;
using UnityEngine;

namespace Gameplay.Entity.Behaviour.Attack
{
    public sealed class AttackBehaviour : Behaviour, IAttackBehaviour
    {
        // Наш актер
        private Stat _damage;
        private Stat _speedAttack;
        
        private IMoveBehaviour _moveBehaviour;
        
        // Остальное
        public IDefenceBehaviour Target { get; private set; }
        
        private CompositeDisposable _attackDisposable;
        private CancellationTokenSource _attackCts;
        
        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);
            
            actor.Stats.TryGetValue(EStat.Damage, out _damage);
            actor.Stats.TryGetValue(EStat.AttackSpeed, out _speedAttack);
            actor.Behaviours.TryGetValue(typeof(IMoveBehaviour), out var moveBehaviour );
            
            _moveBehaviour = moveBehaviour as IMoveBehaviour;
        }
        
        public void CancelAttack()
        {
            _attackCts?.Cancel();
            _attackCts = null;
            _attackDisposable?.Dispose();
            _attackDisposable = null;
            Target =  null;
        }

        public override void SetDisable()
        {
            base.SetDisable();
            
            CancelAttack();
        }

        public void SetTarget(IDefenceBehaviour target)
        {
            if (_damage == null || _speedAttack == null || _moveBehaviour == null)
            {
                Debug.LogError($"[{nameof(AttackBehaviour)}.{nameof(SetTarget)}] Damage or SpeedAttack or MoveBehaviour is null");
                return;
            }
            
            if(Target == target)
                return;
            
            if(!TargetIsValid(target))
                return;
            
            Target = target;
            _attackCts = new();
            _attackDisposable = new();

            BindAlive(target);
            _ = LoopAttack();
        }
        
        private void BindAlive(IDefenceBehaviour target)
        {
            target.Actor.IsAlive
                  .Where(value => value == false)
                  .Subscribe(_ => CancelAttack())
                  .AddTo(_attackDisposable);
        }

        private async UniTaskVoid LoopAttack()
        {
            try
            {
                while (true)
                {
                    // Локальная копия токена, чтобы избежать гонки с CancelAttack
                    var currentCts = _attackCts;
                    if (currentCts == null)
                        return;

                    // Если токен отменён -- выйдем сразу
                    currentCts.Token.ThrowIfCancellationRequested();

                    // Снимем "снимок" текущих ссылок, чтобы избежать гонок с CancelAttack/Disable
                    var currentActor = Actor;
                    var currentTarget = Target;
                    var currentDamage = _damage;
                    var currentSpeedAttack = _speedAttack;
                    var currentMove = _moveBehaviour;

                    // Если что-то стало невалидным, прекращаем атаку
                    if (!ActorIsValid(currentActor)
                        || !TargetIsValid(currentTarget)
                        || currentDamage == null
                        || currentSpeedAttack == null
                        || currentMove == null)
                    {
                        CancelAttack();
                        return;
                    }

                    await currentMove.MoveTo(currentTarget.Actor.Position);
                    await UniTask.Delay(TimeSpan.FromSeconds(currentSpeedAttack.Value), cancellationToken: currentCts.Token);

                    // Повторная проверка после задержки
                    if (ActorIsValid(currentActor)
                        && TargetIsValid(currentTarget)
                        && currentDamage != null
                        && currentSpeedAttack != null
                        && currentMove != null)
                    {
                        currentTarget.TakeDamage(currentDamage.Value);
                        currentActor.Attack.Execute(new AttackContext(currentDamage.Value, currentTarget.Actor));
                    }
                }
            }
            catch (OperationCanceledException)
            {
              
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _moveBehaviour?.CancelMove();
            }
        }

        private bool ActorIsValid(Actor actor)
        {
            return actor != null
                   && actor.IsAlive != null
                   && actor.IsAlive.Value;
        }

        private bool TargetIsValid(IDefenceBehaviour target)
        {
            // Цель должна существовать и быть живой
            return target?.Actor?.IsAlive?.Value == true;
        }
    }
}