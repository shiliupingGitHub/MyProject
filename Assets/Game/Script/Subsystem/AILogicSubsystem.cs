using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Game.Script.AI.Logic;
using Game.Script.Character;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Game.Script.Subsystem
{
    public class AILogicSubsystem : GameSubsystem
    {
        private readonly List<AILogic> _logics = new();
        private List<AICharacter> _characters = new();
        private float _lastTickTime = 0;
        public override void OnInitialize()
        {
            base.OnInitialize();
            var baseType = typeof(AILogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var logic = System.Activator.CreateInstance(type) as AILogic;
                    _logics.Add(logic);
                }
            }

            Common.Game.Instance.addMonster += (character, isServer) =>
            {
                if (isServer)
                {
                    _characters.Add(character);
                }
            };
            
            Common.Game.Instance.removeMonster += (character, isServer) =>
            {
                if (isServer)
                {
                    _characters.Remove(character);
                }
            };
            
            Tick();
        }

        void TickCharacters(float deltaTime)
        {
            foreach (var character in _characters)
            {
                TickLogic(character, deltaTime);
            }
        }

        void TickLogic(AICharacter character, float deltaTime)
        {
            foreach (var logic in _logics)
            {
                logic.Tick(character, deltaTime);
            }
        }

       async void Tick()
        {
            while (true)
            {
                if (_lastTickTime == 0)
                {
                    _lastTickTime = Time.unscaledTime;
                }
                else
                {
                    float curTime = Time.unscaledTime;
                    float delta = curTime - _lastTickTime;
                    _lastTickTime = curTime;
                    TickCharacters(delta);
                }

                await Task.Delay(1);
            }
        }
    }
}