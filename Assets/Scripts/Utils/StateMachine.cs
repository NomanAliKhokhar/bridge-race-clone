using System;
namespace Holex.StateMachine
{
    public class StateMachine
    {
        private IState _currentState;
        private StateTransition[] _normalTransitions;
        private StateTransition[] _anyTransitions;
        public StateMachine(IState defaultState, StateTransition[] normalTransitions = null, StateTransition[] anyTransitions = null)
        {
            _normalTransitions = normalTransitions;
            _anyTransitions = anyTransitions;
            _currentState = defaultState;
            _currentState.OnEnter(); 
        }
        public void Tick()
        {
            StateTransition transition = GetTransition();
            if (transition is not null)
                SetState(transition.To);  // ko�ullar� sa�lam�� bir ge�i� var ona ge�iyoruz!


            _currentState?.Tick(); 
            _currentState.Tick();
        }

        public void SetState(IState state)
        {
            //State de�i�memi�se d�n�yoruz aksi durumunda OnExit() methodlar� �a��r�l�r
            if (state == _currentState) return;

            _currentState?.OnExit(); //aktif stateden ��k�l�yor
            _currentState = state; // yeni state atan�yor
            _currentState.OnEnter(); //state enter methodu �a��r�l�yor

        }
        private StateTransition GetTransition()
        {
            // https://yasirkula.com/2016/06/19/unity-optimizasyon-onerileri/
            // [6] Bir List�in elemanlar�n�n �zerinden for d�ng�s� ile ge�iyorsan�z ve bu List�in eleman say�s� for�dayken de�i�meyecekse,
            // List.Count�u sadece bir kere �a��rmaya �al���n:

            //for (int i = 0; i < _anyTransitions.Length; i++)
            //sanki iki defa de�i�ken tan�mlamam�za gerek yok? -- �ncelik s�ralamas� i�in listeyi yukar�dan a�a�� i�lemiz daha do�ru buna dokunma!
            if (_anyTransitions is not null)
            {
                for (int i = 0, length = _anyTransitions.Length; i < length; i++)
                {
                    if (_anyTransitions[i].Condition()) return _anyTransitions[i];
                }
            }

            if (_normalTransitions is not null)
            {
                for (int i = 0, length = _normalTransitions.Length; i < length; i++)
                {
                    if (_normalTransitions[i].Condition() && // ge�i� ko�ulu sa�lanm��.
                        _normalTransitions[i].From != default &&  // ge�i� yapmak i�in �nceki ko�ul tan�mlanm�� m� ona bak�yoruz
                        _currentState == _normalTransitions[i].From) // ge�erli state den bu ko�ula ge�ilebilir mi?
                    {
                        return _normalTransitions[i]; // her�ey yolundaysa ge�i� i�lemini geri g�nderiyoruz.
                    }
                }
            }
            return null;
        }
    }

    public class StateTransition
    {
        public Func<bool> Condition { get; }
        public IState From { get; }
        public IState To { get; }

        public StateTransition(IState _from, IState _to, Func<bool> condition)
        {
            To = _to;
            From = _from;
            Condition = condition;
        }
    }

    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}