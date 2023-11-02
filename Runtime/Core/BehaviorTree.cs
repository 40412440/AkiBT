using UnityEngine;
using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    public enum UpdateType
    {
        Auto,
        Manual
    }
    [DisallowMultipleComponent]
    /// <summary>
    /// Behavior Tree Component
    /// Awake, Start and Update using UnityEngine's life cycle
    /// </summary>
    public class BehaviorTree : MonoBehaviour, IBehaviorTree
    {

        [HideInInspector, SerializeReference]
        protected Root root = new();
        Object IBehaviorTree._Object => gameObject;
        [HideInInspector, SerializeReference]
        protected List<SharedVariable> sharedVariables = new();
        [SerializeField,
        Tooltip("Switch to UpdateType.Manual to use manual updates and call BehaviorTree.Tick()")]
        private UpdateType updateType;
        [SerializeField, Tooltip("Use the external behavior tree to replace the behavior tree in the component," +
        " and the behavior tree in the component will be overwritten when saving")]
        private BehaviorTreeSO externalBehaviorTree;
#if UNITY_EDITOR
        public BehaviorTreeSO ExternalBehaviorTree => externalBehaviorTree;
        [SerializeField, HideInInspector]
        private List<GroupBlockData> blockData = new();
        public List<GroupBlockData> BlockData => blockData;
#endif
        public Root Root
        {
            get => root;
#if UNITY_EDITOR
            set => root = value;
#endif
        }
        public List<SharedVariable> SharedVariables => sharedVariables;
        private void Awake()
        {
            if (externalBehaviorTree)
            {
                var instance = Instantiate(externalBehaviorTree);
                sharedVariables.Clear();
                sharedVariables.AddRange(instance.SharedVariables);
                root = instance.Root;
            }
            this.MapGlobal();
#if AKIBT_REFLECTION
            SharedVariableMapper.Traverse(this);
#endif
            root.Run(gameObject, this);
            root.Awake();
        }

        private void Start()
        {
            root.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }

        public void Tick()
        {
            root.PreUpdate();
            root.Update();
            root.PostUpdate();
        }

    }
}