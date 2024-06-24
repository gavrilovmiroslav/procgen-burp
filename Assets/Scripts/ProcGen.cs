using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IProcGen
{
    public void Gen();

    void Cleanup();

    void Skip();
}

public abstract class ProcGenStep : MonoBehaviour, IProcGen
{
    public virtual void Cleanup()
    {
    }

    public virtual void Gen()
    {
        ProcGen.Instance.Active = this;
    }

    public void Skip()
    {
        ProcGen.Next();
    }
}

public class ProcGen : MonoBehaviour
{
    public List<UnityEvent> Pipeline;
    public ProcGenStep Active;

    public static ProcGen Instance;

    public void Start()
    {
        Instance = this;
        RunPipeline();
    }

    [HideInInspector]
    public bool StepComplete = false;

    [HideInInspector]
    public int Step = 0;

    public static void Next()
    {
        Instance.StepComplete = true;
    }

    public void RunPipeline()
    {
        StopAllCoroutines();
        Step = 0;
        StartCoroutine(StartPipeline());
    }

    IEnumerator StartPipeline()
    {
        for (Step = 0; Step < Pipeline.Count; Step++)
        {
            (Pipeline[Step].GetPersistentTarget(0) as IProcGen).Cleanup();
        }
            
        for (Step = 0; Step < Pipeline.Count; Step++)
        {
            StepComplete = false;
            Debug.Log("Invoking " + Step);
            Pipeline[Step].Invoke();
            while (!StepComplete)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Proc Gen Pipeline:");
                for (int i = 0; i < Pipeline.Count; i++)
                {
                    var step = Pipeline[i].GetPersistentTarget(0).ToString();
                    var paren = step.IndexOf('(');
                    if (paren >= 0)
                    {
                        step = step.Substring(0, paren);
                    }
                    GUILayout.Label(" " + (i + 1) + ": " + step + (Step == i ? " ←" : ""));
                }
                GUILayout.Space(20);
                if (GUILayout.Button("Restart"))
                {
                    RunPipeline();
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
}
