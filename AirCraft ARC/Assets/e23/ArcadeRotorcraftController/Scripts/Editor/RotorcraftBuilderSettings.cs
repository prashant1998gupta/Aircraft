using System.Collections.Generic;
using UnityEngine;

namespace e23.RotorcraftController.Editor
{
    public class RotorcraftBuilderSettings : ScriptableObject
    {
        [HideInInspector] [SerializeField] private string rotorcraftName;
        [HideInInspector] [SerializeField] private GameObject model;        
        [HideInInspector] [SerializeField] private List<string> rotorBladeNames;
        [HideInInspector] [SerializeField] private RotorcraftBehaviourSettings rotorcraftSettings;
        [HideInInspector] [SerializeField] private bool addEffectsComponent;
        [HideInInspector] [SerializeField] private GameObject trailRendererPrefab;
        [HideInInspector] [SerializeField] private bool addThrustDust;
        [HideInInspector] [SerializeField] private GameObject groundDustParticlePrefab;
        [HideInInspector] [SerializeField] private bool addSimpleAudio;
        [HideInInspector] [SerializeField] private AudioData audioData;
        [HideInInspector] [SerializeField] private bool addGroundFollow;
        [HideInInspector] [SerializeField] private bool addClickToMove;
        [HideInInspector] [SerializeField] private bool addExampleInput;

        public string RotorcraftName { get { return rotorcraftName; } set { rotorcraftName = value; } }
        public GameObject RotorcraftModel { get { return model; } set { model = value; } }
        public List<string> RotorBladeNames { get { return rotorBladeNames; } set { rotorBladeNames = value; } }
        public RotorcraftBehaviourSettings RotorcraftSettings { get { return rotorcraftSettings; } set { rotorcraftSettings = value; } }
        public bool AddEffectsComponent { get { return addEffectsComponent; } set { addEffectsComponent = value; } }
        public GameObject TrailRendererPrefab { get { return trailRendererPrefab; } set { trailRendererPrefab = value; } }
        public bool AddThrustDust { get { return addThrustDust; } set { addThrustDust = value; } }
        public GameObject GroundDustPrefab { get { return groundDustParticlePrefab; } set { groundDustParticlePrefab = value; } }
        public bool AddSimpleAudio { get { return addSimpleAudio; } set { addSimpleAudio = value; } }
        public AudioData AudioData { get { return audioData; } set { audioData = value; } }
        public bool AddGroundFollow { get { return addGroundFollow; } set { addGroundFollow = value; } }
        public bool AddClickToMove { get { return addClickToMove; } set { addClickToMove = value; } }
        public bool AddExampleInput { get { return addExampleInput; } set { addExampleInput = value; } }
    }
}