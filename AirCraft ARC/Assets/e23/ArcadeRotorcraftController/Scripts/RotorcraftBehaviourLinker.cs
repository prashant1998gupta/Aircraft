using UnityEngine;

namespace e23.RotorcraftController
{
    /// <summary>
    /// Add this component to a GameObject for easy linking to a RotorcraftBehaviour.    
    /// You can do other.gameObject.GetComponent<RotorcraftBehaiourLinker>().RotorcrafteBehaviour
    /// </summary>
    public class RotorcraftBehaviourLinker : MonoBehaviour
    {
        [SerializeField] private RotorcraftBehaviour rotorcraftBehaviour = null;

        public RotorcraftBehaviour RotorcraftBehaviour => rotorcraftBehaviour;
    }
}