using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(BouncingBall))]
public class BouncingBallEditor : Editor
{
    public VisualTreeAsset VisualTree;
    private Button randomizeBtn;
    private Button defaultBtn;
    private Button showBallSquash;
    private Button showBallStretch;
    private BouncingBall bouncingBall;

    private void OnEnable()
    {
        bouncingBall = (BouncingBall)target;
    }
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        VisualTree.CloneTree(root);

        randomizeBtn = root.Q<Button>("RandomizeBtn");
        randomizeBtn.RegisterCallback<ClickEvent>(OnRandom);
        defaultBtn = root.Q<Button>("DefaultBtn");
        defaultBtn.RegisterCallback<ClickEvent>(OnDefault);

        var helpBox = new HelpBox("To preview the ball's physics, run the game with adjusted values from the inspector. \nTo preview the ball animation size, hold and release the 'Show Ball' buttons.", HelpBoxMessageType.Info);
        root.Add(helpBox);

        showBallSquash = root.Q<Button>("SquashBtn");
        showBallSquash.RegisterCallback<PointerDownEvent>(e => {
            bouncingBall.ShowBallSquash();
        },TrickleDown.TrickleDown);
        showBallSquash.RegisterCallback<PointerUpEvent>(e => {
            bouncingBall.ResetScales();
        }, TrickleDown.TrickleDown); 
        
        showBallStretch = root.Q<Button>("StretchBtn");
        showBallStretch.RegisterCallback<PointerDownEvent>(e => {
            bouncingBall.ShowBallStretch();
        }, TrickleDown.TrickleDown);
        showBallStretch.RegisterCallback<PointerUpEvent>(e => {
            bouncingBall.ResetScales();
        }, TrickleDown.TrickleDown);


        return root;
    }

    public void OnRandom(ClickEvent evt)
    {
        bouncingBall.AssignRandomVel();
    }

    public void OnDefault(ClickEvent evt)
    {
        bouncingBall.SetDefaultVel();
    }
}
