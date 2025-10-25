using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "ShakeSO", menuName = "Survival Shooter/ShakeSO")]
public class ShakeSO : ScriptableObject
{
    //Scriptable con la configuracion de un ruido para utilizarlo (son de cinemachine)
    public NoiseSettings noiseType;

    //Curva para definir la amplitud a lo largo del tiempo
    public AnimationCurve amplitudCurve;

    //Curva para definir la frecuencia a lo largo del tiempo
    public AnimationCurve frequencyCurve;

    //Duración del shake
    [Range(0f, 4f)]
    public float shakeDuration = 2f;
    
}
