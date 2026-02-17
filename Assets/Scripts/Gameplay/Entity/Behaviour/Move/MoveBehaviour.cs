using Cysharp.Threading.Tasks;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UniRx;
using UnityEngine;

namespace Gameplay.Entity.Behaviour.Move
{
    public sealed class MoveBehaviour : Behaviour, IMoveBehaviour
    {
        // Наш актер
        private Stat _rangeAttack;
        private Stat _speed;

        private Transform _actorTransform;
        
        // Остальное
        private Transform _target;
        
        private CompositeDisposable _disposable;
        private UniTaskCompletionSource _moveTask;

        private float _attackRangeSqr;
        
        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);

            _actorTransform = actor.ActorObject.gameObject.transform;
            
            actor.Stats.TryGetValue(EStat.RangeAttack, out _rangeAttack);
            actor.Stats.TryGetValue(EStat.SpeedMove, out _speed);
        }
        
        public UniTask MoveTo(Transform target)
        {
            if (_rangeAttack == null || _speed == null )
            {
                Debug.LogError($"[{nameof(MoveBehaviour)}.{nameof(MoveTo)}] RangeAttack or Speed is null");
                return UniTask.CompletedTask;
            }
            
            if (target == null)
            {
                Debug.LogError($"[{nameof(MoveBehaviour)}.{nameof(MoveTo)}] Target transform is null");
                return UniTask.CompletedTask;
            }
            
            _attackRangeSqr = _rangeAttack.Value * _rangeAttack.Value;
            
            if ((target.position - _actorTransform.position).sqrMagnitude <= _attackRangeSqr)
            {
                return UniTask.CompletedTask;
            }
            
            _disposable = new();
            _moveTask = new();
            
            _target = target;
            
            Observable.EveryUpdate().Subscribe(Tick).AddTo(_disposable);
            
            return _moveTask.Task;
        }

        private void Tick(long tick)
        {
            if (!ActorsIsAlive())
            {
                CancelMove();
                return;
            }
            
            if ((_target.position - _actorTransform.position).sqrMagnitude <= _attackRangeSqr)
            {
                CancelMove();
                return;
            }
            
            var deltaTime = Time.deltaTime;
            
            _actorTransform.position = Vector3.MoveTowards(
                _actorTransform.position, 
                _target.position, 
                _speed.Value * deltaTime);
            
            if ((_target.position - _actorTransform.position).sqrMagnitude <= _attackRangeSqr)
            {
                CancelMove();
            }
        }

        private bool ActorsIsAlive()
        {
            return Actor != null
                   && Actor.IsAlive.Value
                   && _actorTransform != null
                   && _target != null;
        }

        public void CancelMove()
        {
            _disposable?.Dispose();
            _moveTask?.TrySetResult();
        }
    }
}