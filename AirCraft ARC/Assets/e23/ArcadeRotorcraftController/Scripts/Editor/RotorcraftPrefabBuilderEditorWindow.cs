using System.Collections.Generic;
using e23.Editor;
using e23.RotorcraftController.Examples;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace e23.RotorcraftController.Editor
{
    public class RotorcraftPrefabBuilderEditorWindow : EditorWindow
    {
        [HideInInspector] [SerializeField] private string rotorcraftName = "NewRotorcraft";
        [HideInInspector] [SerializeField] private GameObject rotorcraftModel;                                                                                  

        [SerializeField] public List<string> rotorBladeNames = new List<string>();
                                            
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

        [SerializeField] private RotorcraftBuilderSettings RotorcraftBuilderSettings;

        private Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);

        private Vector2 scrollPos;

        private ScriptableObject scriptableTarget;
        private SerializedObject serializedObject;
        private SerializedProperty rotorBladesSerializedProperty;

        [MenuItem("Tools/e23/ARC/Prefab Builder")]
        private static void Init()
        {
#pragma warning disable 0219
            RotorcraftPrefabBuilderEditorWindow window = (RotorcraftPrefabBuilderEditorWindow)GetWindow(typeof(RotorcraftPrefabBuilderEditorWindow), false, "ARC - Prefab Builder");
#pragma warning restore 0219
        }

        private void OnEnable()
        {
            FindPrefabBuilderSettings();
            SetupSerializedObject();
        }

        private void OnDisable()
        {
            SavePrefabSetup();
        }

        private void FindPrefabBuilderSettings()
        {
            string settingsType = "t:" + nameof(Editor.RotorcraftBuilderSettings);
            string[] guids = AssetDatabase.FindAssets(settingsType);
            
            if (guids.Length == 0)
            {
                RotorcraftBuilderSettings newSettings = ScriptableObject.CreateInstance<RotorcraftBuilderSettings>();
                AssetDatabase.CreateAsset(newSettings, "Assets/e23/ArcadeRotorcraftController/Scripts/Editor/RotorcraftBuilderSettings.asset");
                RotorcraftBuilderSettings = newSettings;

                SavePrefabSetup();
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                RotorcraftBuilderSettings rbs = (RotorcraftBuilderSettings)AssetDatabase.LoadAssetAtPath(path, typeof(RotorcraftBuilderSettings));

                RotorcraftBuilderSettings = rbs;
                LoadPrefabSetup();
            }
        }

        private void SetupSerializedObject()
        {
            scriptableTarget = this;
            serializedObject = new SerializedObject(scriptableTarget);
            rotorBladesSerializedProperty = serializedObject.FindProperty("rotorBladeNames");
        }

        private void OnGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Build Rotorcraft", GUILayout.MinHeight(100), GUILayout.Height(50)))
            {
                CreateRotorcraft();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 75f));

            EditorBoilerPlate.CreateLabelField("Rotorcraft", true);
            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Rotorcraft Name");
            rotorcraftName = EditorGUILayout.TextField(rotorcraftName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Rotorcraft Model");
            rotorcraftModel = (GameObject)EditorGUILayout.ObjectField(rotorcraftModel, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(rotorBladesSerializedProperty, true);
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(scriptableTarget, "Changed rotor blade names");

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Rotorcraft Settings");
            rotorcraftSettings = (RotorcraftBehaviourSettings)EditorGUILayout.ObjectField(rotorcraftSettings, typeof(RotorcraftBehaviourSettings), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorBoilerPlate.DrawSeparatorLine();

            EditorBoilerPlate.CreateLabelField("Optional Components", true);
            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Add Effects Component");
            addEffectsComponent = EditorGUILayout.Toggle("", addEffectsComponent);
            EditorGUILayout.EndHorizontal();

            if (addEffectsComponent == true)
            {
                DisplayEffects();
            }
            
            EditorBoilerPlate.DrawSeparatorLine();

            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Add Dust Effects");
            addThrustDust = EditorGUILayout.Toggle("", addThrustDust);
            EditorGUILayout.EndHorizontal();

            if (addThrustDust == true)
            {
                DisplayDust();
            }

            EditorBoilerPlate.DrawSeparatorLine();

            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Add Simple Audio");
            addSimpleAudio = EditorGUILayout.Toggle("", addSimpleAudio);
            EditorGUILayout.EndHorizontal();

            if (addSimpleAudio == true)
            {
                DisplayAudio();
            }
            
            EditorBoilerPlate.DrawSeparatorLine();

            EditorBoilerPlate.CreateLabelField("Movement", true);
            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Add Ground Follow");
            addGroundFollow = EditorGUILayout.Toggle("", addGroundFollow);
            EditorGUILayout.EndHorizontal();

            if (addGroundFollow == true)
            {
                DisplayMovementInfo();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Add Click To Move");
            addClickToMove = EditorGUILayout.Toggle("", addClickToMove);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Add Example Input");
            addExampleInput = EditorGUILayout.Toggle("", addExampleInput);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
        }

        private void DisplayEffects()
        {
            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Trail Renderer Prefab");
            trailRendererPrefab = (GameObject)EditorGUILayout.ObjectField(trailRendererPrefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void DisplayDust()
        {
            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Dust Particles Prefab");
            groundDustParticlePrefab = (GameObject)EditorGUILayout.ObjectField(groundDustParticlePrefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void DisplayAudio()
        {
            EditorGUILayout.BeginHorizontal();
            EditorBoilerPlate.CreateLabelField("Audio Data");
            audioData = (AudioData)EditorGUILayout.ObjectField(audioData, typeof(AudioData), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void DisplayMovementInfo()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("When using the GroundFollow component, it is not recommended to allow the rotorcraft to change height via Throttle()", MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void CreateRotorcraft()
        {
            GameObject rotorcraftParent = new GameObject(rotorcraftName);
            UpdateTransform(rotorcraftParent.transform, null, Vector3.zero, defaultRotation);

            Transform parentTransform = CreateRigidbodyGameObject();

            GameObject rotorcraftBehaviour = new GameObject("Rotorcraft");
            SetupRotorcraftBehaviour(rotorcraftBehaviour, parentTransform);

            Rigidbody srb = parentTransform.GetComponent<Rigidbody>();

            GameObject newRotorcraftModel = (GameObject) PrefabUtility.InstantiatePrefab(rotorcraftModel);
            SetupModel(newRotorcraftModel, rotorcraftBehaviour.transform);

            RotorcraftBehaviour vb = rotorcraftBehaviour.GetComponent<RotorcraftBehaviour>();
            TryFindParts(vb, newRotorcraftModel, srb);

            CreateBoxCollider(newRotorcraftModel);
            if (addEffectsComponent == true)
            {
                CreateEffectsObjects(newRotorcraftModel.transform);
            }

            if (addThrustDust == true)
            {
                if (groundDustParticlePrefab == null)
                {
                    Debug.LogWarning($"ARC - Dust particle is empty on Prefab Builder Window, ignoring option.");
                }
                else
                {                
                    RotorcraftDustEffect rde = rotorcraftBehaviour.AddComponent<RotorcraftDustEffect>();
                    rde.AssignEffect(CreateDustEffectObjects(parentTransform));
                }
            }

            Selection.activeObject = rotorcraftBehaviour;
            Undo.RegisterCreatedObjectUndo(rotorcraftParent, "ARC Rotorcraft Created");

            Transform CreateRigidbodyGameObject()
            {
                GameObject rbObject = new GameObject("Rigidbody");
                rbObject.transform.SetParent(rotorcraftParent.transform);

                Rigidbody rb = rbObject.AddComponent<Rigidbody>();
                rb.mass = 100;
                rb.drag = 2;
                rb.angularDrag = 0.05f;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                return rbObject.transform;
            }
        }

        private void SetupRotorcraftBehaviour(GameObject obj, Transform parent)
        {
            UpdateTransform(obj.transform, parent, Vector3.zero, defaultRotation);

            obj.AddComponent<RotorcraftBehaviour>();
            
            if (addEffectsComponent == true)
            {
                obj.AddComponent<RotorcraftEffects>();
            }

            if (addSimpleAudio == true)
            {
                RotorcraftAudio ra = obj.AddComponent<RotorcraftAudio>();
                ra.SetAudioData(audioData);
            }

            if (addGroundFollow == true)
            {
                RotorcraftGroundFollow groundFollow = obj.AddComponent<RotorcraftGroundFollow>();
            }

            if (addClickToMove == true)
            {
                RotorcraftClickToMove clickToMove = obj.AddComponent<RotorcraftClickToMove>();
            }

            if (addExampleInput == true)
            {
                ExampleInput exInput = obj.AddComponent<ExampleInput>();
                exInput.RotorcraftBehaviour = obj.GetComponent<RotorcraftBehaviour>();
            }
        }

        private void SetupModel(GameObject obj, Transform parent)
        {
            if (obj == null)
            {
                Debug.Log("No model prefab has been assigned, only the skeleton will be created");
                return;
            }

            obj.name = rotorcraftModel.name;
            obj.transform.localScale = Vector3.one;

            UpdateTransform(obj.transform, parent, Vector3.zero, defaultRotation);
        }

        private void TryFindParts(RotorcraftBehaviour rotorcraftBehaviour, GameObject obj, Rigidbody rigidBody)
        {
            if (obj == null)
            {
                return;
            }

            rotorcraftBehaviour.RotorcraftModel = obj.transform;
            rotorcraftBehaviour.RotorcraftRigidbody = rigidBody;

            foreach (var blade in rotorBladeNames)
            {
                Transform bladeTrans = SearchForPart(rotorcraftBehaviour.transform, blade);
                bladeTrans.gameObject.AddComponent<RotorBlade>();
            }

            if (rotorcraftSettings != null)
            {
                rotorcraftBehaviour.RotorcraftSettings = rotorcraftSettings;
            }
            else
            {
                Debug.Log("No Rotorcraft Settings have been added, to create one: Right mouse click in the project window -> create -> e23 -> Rotorcraft Settings. Then assign the asset.");
            }
        }

        private Transform SearchForPart(Transform parent, string part)
        {
            foreach(Transform t in parent.GetComponentsInChildren<Transform>())
            {
                string name = t.name.ToLower();
                if (name.Contains(part.ToLower()))
                {
                    return t;
                }
            }
            
            return null;
        }

        private void CreateEffectsObjects(Transform parent)
        {
            if (trailRendererPrefab != null)
            {
                GameObject trailParent = new GameObject("TrailsParent");
                UpdateTransform(trailParent.transform, parent, Vector3.zero, defaultRotation);

                GameObject newtrailRenderer = (GameObject) PrefabUtility.InstantiatePrefab(trailRendererPrefab);
                newtrailRenderer.name = trailRendererPrefab.name;
                Vector3 trailPos = Vector3.zero;
                    
                UpdateTransform(newtrailRenderer.transform, trailParent.transform, trailPos, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            }       
        }

        private ParticleSystem CreateDustEffectObjects(Transform parent)
        {                        
            GameObject newDustParticles = (GameObject)PrefabUtility.InstantiatePrefab(groundDustParticlePrefab);
            newDustParticles.name = groundDustParticlePrefab.name;
            UpdateTransform(newDustParticles.transform, parent, Vector3.zero, Quaternion.identity);

            return newDustParticles.GetComponent<ParticleSystem>();
        }

        private void UpdateTransform(Transform obj, Transform parent, Vector3 pos, Quaternion rot)
        {
            obj.SetParent(parent);
            obj.localPosition = pos;
            obj.localRotation = rot;
        }

        private void CreateBoxCollider(GameObject model)
        {
            Transform boxParent = model.transform;

            GameObject boxCollider = new GameObject("RotorcraftCollider");
            boxCollider.transform.SetParent(boxParent);
            boxCollider.AddComponent<BoxCollider>();

            Bounds bodyBounds = boxParent.GetComponentInChildren<Renderer>().bounds;
            var renderers = boxParent.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                bodyBounds.Encapsulate(renderer.bounds);
            }

            boxCollider.transform.localScale = bodyBounds.size;
            Vector3 boxPos = new Vector3(boxCollider.transform.localPosition.x, bodyBounds.center.y, boxCollider.transform.localPosition.z);
            boxCollider.transform.localPosition = boxPos;
        }

        private void LoadPrefabSetup()
        {
            rotorcraftName = RotorcraftBuilderSettings.RotorcraftName;
            rotorcraftModel = RotorcraftBuilderSettings.RotorcraftModel;
            rotorBladeNames = RotorcraftBuilderSettings.RotorBladeNames;
            rotorcraftSettings = RotorcraftBuilderSettings.RotorcraftSettings;
            addEffectsComponent = RotorcraftBuilderSettings.AddEffectsComponent;
            trailRendererPrefab = RotorcraftBuilderSettings.TrailRendererPrefab;
            addThrustDust = RotorcraftBuilderSettings.AddThrustDust;
            groundDustParticlePrefab = RotorcraftBuilderSettings.GroundDustPrefab;
            addSimpleAudio = RotorcraftBuilderSettings.AddSimpleAudio;
            audioData = RotorcraftBuilderSettings.AudioData;
            addGroundFollow = RotorcraftBuilderSettings.AddGroundFollow;
            addClickToMove = RotorcraftBuilderSettings.AddClickToMove;
            addExampleInput = RotorcraftBuilderSettings.AddExampleInput;
        }

        private void SavePrefabSetup()
        {
            RotorcraftBuilderSettings.RotorcraftName = rotorcraftName;
            RotorcraftBuilderSettings.RotorcraftModel = rotorcraftModel;
            RotorcraftBuilderSettings.RotorBladeNames = rotorBladeNames;
            RotorcraftBuilderSettings.RotorcraftSettings = rotorcraftSettings;
            RotorcraftBuilderSettings.AddEffectsComponent = addEffectsComponent;
            RotorcraftBuilderSettings.TrailRendererPrefab = trailRendererPrefab;
            RotorcraftBuilderSettings.AddThrustDust = addThrustDust;
            RotorcraftBuilderSettings.GroundDustPrefab = groundDustParticlePrefab;
            RotorcraftBuilderSettings.AddSimpleAudio = addSimpleAudio;
            RotorcraftBuilderSettings.AudioData = audioData;
            RotorcraftBuilderSettings.AddGroundFollow = addGroundFollow;
            RotorcraftBuilderSettings.AddClickToMove = addClickToMove;
            RotorcraftBuilderSettings.AddExampleInput = addExampleInput;

            if (RotorcraftBuilderSettings != null)
            {
                EditorUtility.SetDirty(RotorcraftBuilderSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}