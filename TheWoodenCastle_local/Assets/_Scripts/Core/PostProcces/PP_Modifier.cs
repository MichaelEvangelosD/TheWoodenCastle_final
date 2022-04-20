using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/* CLASS DOCUMENTATION *\
 * [Must Know]
 * This is an abstract class and is inherited from ChromaticController and LensDistorter scripts
 */

public abstract class PP_Modifier : MonoBehaviour
{
    protected PostProcessVolume mainPPVolume;

    protected abstract void Awake();

    protected abstract void Start();

    protected abstract void Update();

    protected abstract void LowerValue();

    protected abstract void OnDestroy();
}
