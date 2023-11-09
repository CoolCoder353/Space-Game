using NaughtyAttributes;
using UnityEngine;


public class Curve_formula_calculator : MonoBehaviour
{
    // Declare a public AnimationCurve variable that can be edited in the inspector
    public AnimationCurve curve;

    public bool runInGame = false;

    // Declare a private float variable to store the current time
    private float time;

    [Button("Calculate Curve")]
    void Calculate()
    {
        // Initialize the time variable to zero
        time = 0f;

        // Get the keyframes of the curve
        Keyframe[] keys = curve.keys;

        // Loop through the keyframes and print the formula of each segment
        for (int i = 0; i < keys.Length - 1; i++)
        {
            // Get the current and next keyframe
            Keyframe k1 = keys[i];
            Keyframe k2 = keys[i + 1];

            // Get the time and value of each keyframe
            float t1 = k1.time;
            float t2 = k2.time;
            float v1 = k1.value;
            float v2 = k2.value;

            // Get the in and out tangents of each keyframe
            float m1 = k1.outTangent;
            float m2 = k2.inTangent;

            // Calculate the coefficients of the cubic polynomial that defines the segment
            float a = 2f * (v1 - v2) + (m1 + m2) * (t2 - t1);
            float b = -3f * (v1 - v2) - (m1 + 2f * m2) * (t2 - t1);
            float c = m1 * (t2 - t1);
            float d = v1;

            // Print the formula of the segment
            Debug.Log("The formula of the segment between time " + t1 + " and " + t2 + " is: ");
            Debug.Log("y=" + a + " * (t - " + t1 + ")³ + " + b + " * (t - " + t1 + ")² + " + c + " * (t - " + t1 + ") + " + d);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (runInGame) { Calculate(); }

    }

    // Update is called once per frame
    void Update()
    {
        if (runInGame)
        {
            // Increment the time variable by the delta time
            time += Time.deltaTime;

            // Evaluate the curve at the current time and print the result
            float value = curve.Evaluate(time);
            Debug.Log("The value of the curve at time " + time + " is " + value);
        }
    }
}