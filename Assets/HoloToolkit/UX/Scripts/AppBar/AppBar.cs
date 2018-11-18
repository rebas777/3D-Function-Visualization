// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Logic for the App Bar. Generates buttons, manages states.
    /// </summary>
    public class AppBar : InteractionReceiver
    {
        private float buttonWidth = 1.50f;

        /// <summary>
        /// How many custom buttons can be added to the toolbar
        /// </summary>
        public const int MaxCustomButtons = 5;

        /// <summary>
        /// Where to display the app bar on the y axis
        /// This can be set to negative values
        /// to force the app bar to appear below the object
        /// </summary>
        public float HoverOffsetYScale = 0.25f;

        /// <summary>
        /// Pushes the app bar away from the object
        /// </summary>
        public float HoverOffsetZ = 0f;

        /// <summary>
        /// Class used for building toolbar buttons
        /// (not yet in use)
        /// </summary>
        [Serializable]
        public struct ButtonTemplate
        {
            public ButtonTemplate(ButtonTypeEnum type, string name, string icon, string text, int defaultPosition, int manipulationPosition)
            {
                Type = type;
                Name = name;
                Icon = icon;
                Text = text;
                DefaultPosition = defaultPosition;
                ManipulationPosition = manipulationPosition;
                EventTarget = null;
                OnTappedEvent = null;
            }

            public bool IsEmpty
            {
                get
                {
                    return string.IsNullOrEmpty(Name);
                }
            }

            public int DefaultPosition;
            public int ManipulationPosition;
            public ButtonTypeEnum Type;
            public string Name;
            public string Icon;
            public string Text;
            public InteractionReceiver EventTarget;
            public UnityEvent OnTappedEvent;
        }

        [Flags]
        public enum ButtonTypeEnum
        {
            Custom = 0,
            Remove = 1,
            Adjust = 2,
            Hide = 4,
            Show = 8,
            Done = 16
        }

        public enum AppBarDisplayTypeEnum
        {
            Manipulation,
            Standalone
        }

        public enum AppBarStateEnum
        {
            Default,
            Manipulation,
            Hidden
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
            set
            {
                boundingBox = value;
            }
        }

        public GameObject SquareButtonPrefab;

        public int NumDefaultButtons
        {
            get
            {
                return numDefaultButtons;
            }
        }

        public int NumManipulationButtons
        {
            get
            {
                return numManipulationButtons;
            }
        }

        public bool UseRemove = true;
        public bool UseAdjust = true;
        public bool UseHide = true;

        public ButtonTemplate[] Buttons
        {
            get
            {
                return buttons;
            }
            set
            {
                buttons = value;
            }
        }

        public ButtonTemplate[] DefaultButtons
        {
            get
            {
                return defaultButtons;
            }
        }

        public AppBarDisplayTypeEnum DisplayType = AppBarDisplayTypeEnum.Manipulation;
        public AppBarStateEnum State = AppBarStateEnum.Default;

        /// <summary>
        /// Custom icon profile
        /// If null, the profile in the SquareButtonPrefab object will be used
        /// </summary>
        public ButtonIconProfile CustomButtonIconProfile;

        [SerializeField]
        private ButtonTemplate[] buttons = new ButtonTemplate[MaxCustomButtons];

        [SerializeField]
        private Transform buttonParent;

        [SerializeField]
        private GameObject baseRenderer;

        [SerializeField]
        private GameObject backgroundBar;

        [SerializeField]
        private BoundingBox boundingBox;

        private ButtonTemplate[] defaultButtons;
        private Vector3[] forwards = new Vector3[4];
        private Vector3 targetBarSize = Vector3.one;
        private float lastTimeTapped = 0f;
        private float coolDownTime = 0.5f;
        private int numDefaultButtons;
        private int numHiddenButtons;
        private int numManipulationButtons;

        /*****************************self-defined*****************************/

        private GameObject manager;
        private GameObject shaderMenu;
        private bool shaderMenuShow = false;

        private GameObject inputPanel;
        private bool inputPanelShow = false;

        private bool analysisValueShow = false;

        private GameObject valuePanel;
        private bool observeModeOn = false;

        private Transform cameraTransform;
        private InputField inputField;

        private GameObject generatedMesh;
        private GameObject upSurf;
        private GameObject downSurf;

        private GameObject photoCanvas;
        private GameObject screenCapture;

        public ParticleSystem particlesys;
        public ParticleIllustration pi;
        private CalculatorWrapper cw;

        private GameObject inputMenu;
        private GameObject implicitMenu;
        private GameObject captureMenu;
        

            
            
        /**********************************************************************/

        public void Reset()
        {
            State = AppBarStateEnum.Default;
            FollowBoundingBox(false);
            lastTimeTapped = Time.time + coolDownTime;
        }

        public void Start()
        {
            State = AppBarStateEnum.Default;
            if (interactables.Count == 0)
            {
                RefreshTemplates();
                for (int i = 0; i < defaultButtons.Length; i++)
                {
                    CreateButton(defaultButtons[i], null);
                }

                for (int i = 0; i < buttons.Length; i++)
                {
                    CreateButton(buttons[i], CustomButtonIconProfile);
                }
            }

            /******************self-defined***************/
            manager = GameObject.Find("manager");
            cameraTransform = GameObject.Find("MixedRealityCamera").GetComponent<Transform>();
            inputField = GameObject.Find("InputField").GetComponent<HoloToolkit.UI.Keyboard.KeyboardInputField>();
            generatedMesh = GameObject.Find("GeneratedMesh");
            upSurf = GameObject.Find("UpSurf");
            upSurf = GameObject.Find("DownSurf");


            shaderMenu = GameObject.Find("ShaderButtons");
            shaderMenu.SetActive(false);

            inputPanel = GameObject.Find("KeyboardCanvas");
            inputPanel.SetActive(false);

            valuePanel = GameObject.Find("ValueCanvas");
            valuePanel.SetActive(false);

            photoCanvas = GameObject.Find("PhotoCanvas");
            photoCanvas.SetActive(false);

            screenCapture = GameObject.Find("ScreenCapture");

            inputMenu = GameObject.Find("InputButtons");
            inputMenu.SetActive(false);

            implicitMenu = GameObject.Find("ImplicitButtons");
            implicitMenu.SetActive(false);

            captureMenu = GameObject.Find("CaptureButtons");
            captureMenu.SetActive(false);

            particlesys = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
            pi = new ParticleIllustration(particlesys);
            cw = GameObject.Find("manager").GetComponent<CalculatorWrapper>();

        }

        protected override void InputClicked(GameObject obj, InputClickedEventData eventData)
        {
            if (Time.time < lastTimeTapped + coolDownTime)
            {
                return;
            }

            lastTimeTapped = Time.time;
            base.InputClicked(obj, eventData);

            switch (obj.name)
            {
                case "Remove":
                    // Destroy the target object, Bounding Box, Bounding Box Rig and App Bar
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
                    Destroy(boundingBox.Target.GetComponent<BoundingBoxRig>());
                    Destroy(boundingBox.Target);
                    Destroy(gameObject);
                    break;

                case "Adjust":
                    // Make the bounding box active so users can manipulate it
                    State = AppBarStateEnum.Manipulation;
                    // Activate BoundingBoxRig
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Activate();
                    break;

                case "Hide":
                    // Make the bounding box inactive and invisible
                    State = AppBarStateEnum.Hidden;
                    break;

                case "Show":
                    State = AppBarStateEnum.Default;
                    // Deactivate BoundingBoxRig
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
                    break;

                case "Done":
                    State = AppBarStateEnum.Default;
                    // Deactivate BoundingBoxRig
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
                    break;

                default:
                    break;
            }
        }

        private void CreateButton(ButtonTemplate template, ButtonIconProfile customIconProfile)
        {
            if (template.IsEmpty)
            {
                return;
            }

            switch (template.Type)
            {
                case ButtonTypeEnum.Custom:
                    numDefaultButtons++;
                    break;

                case ButtonTypeEnum.Adjust:
                    numDefaultButtons++;
                    break;

                case ButtonTypeEnum.Done:
                    numManipulationButtons++;
                    break;

                case ButtonTypeEnum.Remove:
                    numManipulationButtons++;
                    numDefaultButtons++;
                    break;

                case ButtonTypeEnum.Hide:
                    numDefaultButtons++;
                    break;

                case ButtonTypeEnum.Show:
                    numHiddenButtons++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameObject newButton = Instantiate(SquareButtonPrefab, buttonParent);
            newButton.name = template.Name;
            newButton.transform.localPosition = Vector3.zero;
            newButton.transform.localRotation = Quaternion.identity;
            AppBarButton mtb = newButton.AddComponent<AppBarButton>();
            mtb.Initialize(this, template, customIconProfile);
        }

        private void FollowBoundingBox(bool smooth)
        {
            if (boundingBox == null)
            {
                if (DisplayType == AppBarDisplayTypeEnum.Manipulation)
                {
                    // Hide our buttons
                    baseRenderer.SetActive(false);
                }
                else
                {
                    baseRenderer.SetActive(true);
                }
                return;
            }

            // Show our buttons
            baseRenderer.SetActive(true);

            // Get positions for each side of the bounding box
            // Choose the one that's closest to us
            forwards[0] = boundingBox.transform.forward;
            forwards[1] = boundingBox.transform.right;
            forwards[2] = -boundingBox.transform.forward;
            forwards[3] = -boundingBox.transform.right;
            Vector3 scale = boundingBox.TargetBoundsLocalScale;
            float maxXYScale = Mathf.Max(scale.x, scale.y);
            float closestSoFar = Mathf.Infinity;
            Vector3 finalPosition = Vector3.zero;
            Vector3 finalForward = Vector3.zero;
            Vector3 headPosition = Camera.main.transform.position;

            for (int i = 0; i < forwards.Length; i++)
            {
                Vector3 nextPosition = boundingBox.transform.position +
                (forwards[i] * -maxXYScale) +
                (Vector3.up * (-scale.y * HoverOffsetYScale));

                float distance = Vector3.Distance(nextPosition, headPosition);
                if (distance < closestSoFar)
                {
                    closestSoFar = distance;
                    finalPosition = nextPosition;
                    finalForward = forwards[i];
                }
            }

            // Apply hover offset
            finalPosition += (finalForward * -HoverOffsetZ);

            // Follow our bounding box
            transform.position = smooth ?
                Vector3.Lerp(transform.position, finalPosition, 0.5f) :
                finalPosition;

            // Rotate on the y axis
            Vector3 eulerAngles = Quaternion.LookRotation((boundingBox.transform.position - finalPosition).normalized, Vector3.up).eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;
        }

        private void Update()
        {
            Vector3 oldPos = this.transform.position;
            FollowBoundingBox(true);
            Vector3 expectedPos = this.transform.position;
            if(Vector3.Distance(expectedPos, generatedMesh.transform.position)
                > generatedMesh.transform.localScale.x * 18.0f) {
                this.transform.position = oldPos;
            }



            switch(State)
            {
                case AppBarStateEnum.Default:
                    targetBarSize = new Vector3(numDefaultButtons * buttonWidth, buttonWidth, 1f);
                    break;

                case AppBarStateEnum.Hidden:
                    targetBarSize = new Vector3(numHiddenButtons * buttonWidth, buttonWidth, 1f);
                    break;

                case AppBarStateEnum.Manipulation:
                    targetBarSize = new Vector3(numManipulationButtons * buttonWidth, buttonWidth, 1f);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            backgroundBar.transform.localScale = Vector3.Lerp(backgroundBar.transform.localScale, targetBarSize, 0.5f);

            if(Input.GetKeyDown(KeyCode.P)) {
                Calculator4D catmp = new Calculator4D(-5, -5, -5, 5, 5, 5, 0.1f);
                //catmp.calculate("z*z+2xyz-16");
                cw.calculate("z*z+2xyz-16");
            }
            if(Input.GetKeyDown(KeyCode.O)) {
                Calculator4D catmp = new Calculator4D(-5, -5, -5, 5, 5, 5, 0.1f);
                //catmp.calculate("z*z+2xyz-16");
                cw.calculatevec("pow(x,3)+pow(y,3)-6xy");
            }
            List<List<float>> cwvecret;
            if((cwvecret = cw.getCalcuResultvec()) != null) {
                pi.initialParticle();

                for(int i = 0; i <= cw.sampleFreqy*10-1; i += 1) {
                    for(int j = 0; j <= cw.sampleFreqx*10-1; j += 1) {
                        //Debug.Log("herej" + j);
                        if(float.IsNaN(cwvecret[i][j])) {
                            continue;
                        }
                        float devi = Mathf.Abs(cwvecret[i][j]);

                        if(devi <= cw.gap/10 * 25) {

                            if(devi <= Mathf.Abs(cwvecret[i][j - 1 >= 0 ? (j - 1) : 0]) && devi <= Mathf.Abs(cwvecret[i][j + 1 < cw.sampleFreqx * 10 ? (j + 1) : cw.sampleFreqx * 10 - 1])) {
                                Vector3 posi = new Vector3((cw.xMin + cw.gap/10 * i),  (cw.yMin + cw.gap/10 * j),0);
                                pi.drawParticle(posi);
                            }
                            else if(devi <= Mathf.Abs(cwvecret[i - 1 >= 0 ? (i - 1) : 0][j]) && devi <= Mathf.Abs(cwvecret[i + 1 < cw.sampleFreqy * 10 ? (i + 1) : cw.sampleFreqy * 10 - 1][j])) {
                                Vector3 posi = new Vector3((cw.xMin + cw.gap/10 * i),  (cw.yMin + cw.gap/10 * j),0);
                                pi.drawParticle(posi);
                            }


                        }
                    }

                }

                pi.endDraw();
            }

            List<List<List<float>>> cwret;
            if((cwret = cw.getCalcuResult()) != null) {
                //todo destroy the inform
                Debug.Log("here");
               
                pi.initialParticle();

                for(int i = 2; i <= cw.sampleFreqz - 2; i += 1) {
                    for(int j = 2; j <= cw.sampleFreqy - 2; j += 1) {
                        for(int k = 2; k < cw.sampleFreqx - 2; k++) {
                            float devi = Mathf.Abs(cwret[i][j][k]);
                            if(float.IsNaN(cwret[i][j][k])) {
                                continue;
                            }

                            if(devi <= cw.gap * 4) {

                                if(devi <= Mathf.Abs(cwret[i][j][k - 1 >= 0 ? (k - 1) : 0]) && devi <= Mathf.Abs(cwret[i][j][k + 1 < cw.sampleFreqx ? (k + 1) : cw.sampleFreqx - 1])) {
                                    Vector3 posi = new Vector3((cw.xMin + cw.gap * i), (cw.zMin + cw.gap * k), (cw.yMin + cw.gap * j));
                                    pi.drawParticle( posi);
                                }
                                else if(devi <= Mathf.Abs(cwret[i][j - 1 >= 0 ? (j - 1) : 0][k]) && devi <= Mathf.Abs(cwret[i][j + 1 < cw.sampleFreqy ? (j + 1) : cw.sampleFreqy - 1][k])) {
                                    Vector3 posi = new Vector3((cw.xMin + cw.gap * i), (cw.zMin + cw.gap * k), (cw.yMin + cw.gap * j));
                                    pi.drawParticle( posi);
                                }
                                else if(devi <= Mathf.Abs(cwret[i - 1 >= 0 ? (i - 1) : 0][j][k]) && devi <= Mathf.Abs(cwret[i + 1 < cw.sampleFreqz ? (i + 1) : cw.sampleFreqz - 1][j][k])) {
                                    Vector3 posi = new Vector3((cw.xMin + cw.gap * i), (cw.zMin + cw.gap * k), (cw.yMin + cw.gap * j));
                                    pi.drawParticle(posi);
                                }
                            }
                        }
                    }

                }
          
                pi.endDraw();
                
            }


        }

        private void RefreshTemplates()
        {
            int numCustomButtons = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (!buttons[i].IsEmpty)
                {
                    numCustomButtons++;
                }
            }

            var defaultButtonsList = new List<ButtonTemplate>();

            // Create our default button templates based on user preferences
            if (UseRemove)
            {
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Remove, numCustomButtons, UseHide, UseAdjust));
            }

            if (UseAdjust)
            {
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Adjust, numCustomButtons, UseHide, UseAdjust));
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Done, numCustomButtons, UseHide, UseAdjust));
            }

            if (UseHide)
            {
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Hide, numCustomButtons, UseHide, UseAdjust));
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Show, numCustomButtons, UseHide, UseAdjust));
            }
            defaultButtons = defaultButtonsList.ToArray();
        }

#if UNITY_EDITOR
        public void EditorRefreshTemplates()
        {
            RefreshTemplates();
        }
#endif

        /// <summary>
        /// Generates a template for a default button based on type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="numCustomButtons"></param>
        /// <param name="useHide"></param>
        /// <param name="useAdjust"></param>
        /// <returns></returns>
        private static ButtonTemplate GetDefaultButtonTemplateFromType(ButtonTypeEnum type, int numCustomButtons, bool useHide, bool useAdjust)
        {
            // Button position is based on custom buttons
            // In the app bar, Hide/Show
            switch (type)
            {
                case ButtonTypeEnum.Custom:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Custom,
                        "Custom",
                        "",
                        "Custom",
                        0,
                        0);

                case ButtonTypeEnum.Adjust:
                    int adjustPosition = numCustomButtons + 1;

                    if (!useHide)
                    {
                        adjustPosition--;
                    }

                    return new ButtonTemplate(
                        ButtonTypeEnum.Adjust,
                        "Adjust",
                        "ObjectCollectionScatter", // Replace with your custom icon texture name in HolographicButton prefab
                        "Adjust",
                        adjustPosition, // Always next-to-last to appear
                        0);

                case ButtonTypeEnum.Done:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Done,
                        "Done",
                        "ObjectCollectionScatter", // Replace with your custom icon texture name in HolographicButton prefab
                        "Done",
                        0,
                        0);

                case ButtonTypeEnum.Hide:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Hide,
                        "Hide",
                        "ObjectCollectionScatter", // Replace with your custom icon texture name in HolographicButton prefab
                        "Hide Menu",
                        0, // Always the first to appear
                        0);

                case ButtonTypeEnum.Remove:
                    int removePosition = numCustomButtons + 1;
                    if (useAdjust)
                    {
                        removePosition++;
                    }

                    if (!useHide)
                    {
                        removePosition--;
                    }

                    return new ButtonTemplate(
                        ButtonTypeEnum.Remove,
                        "Remove",
                        "ObjectCollectionScatter", // Replace with your custom icon texture name in HolographicButton prefab
                        "Remove",
                        removePosition, // Always the last to appear
                        1);

                case ButtonTypeEnum.Show:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Show,
                        "Show",
                        "ObjectCollectionScatter", // Replace with your custom icon texture name in HolographicButton prefab
                        "Show Menu",
                        0,
                        0);

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        protected override void FocusEnter(GameObject obj, PointerSpecificEventData eventData) {
            //Debug.Log(obj.name + " : FocusEnter");
        }

        protected override void FocusExit(GameObject obj, PointerSpecificEventData eventData) {
            //Debug.Log(obj.name + " : FocusExit");
        }

        protected override void InputDown(GameObject obj, InputEventData eventData) {
            //Debug.Log(obj.name + " : InputDown");
            switch(obj.name) {
                case "input": {
                        inputPanelShowOrHide();
                        break;
                    }
                case "shaders": {
                        shaderMenuShowOrHide();
                        break;
                    }
                case "point": {
                        pointObserverShowOrHide();
                        break;
                    }
                case "value": {
                        valueAnalysisShowOrHide();
                        break;
                    }
                case "hand": {
                        handwritingStart();
                        break;
                    }
                case "implicit": {
                        implicitInpuPanelShowOrHide();
                        break;
                    }
            }
        }

        protected override void InputUp(GameObject obj, InputEventData eventData) {
            //Debug.Log(obj.name + " : InputUp");
        }

        private void shaderMenuShowOrHide() {
            if(shaderMenuShow) {
                shaderMenuShow = false;
                shaderMenu.SetActive(false);
            }
            else {
                // show the shader menu
                shaderMenuShow = true;
                shaderMenu.SetActive(true);
                // and adjust the shader menu's position and rotation according to the camera
                objSuspendInFrontOfCam(shaderMenu);
            }
        }

        private void inputPanelShowOrHide() {
            if(inputPanelShow) {
                inputPanelShow = false;
                inputPanel.SetActive(false);
                inputMenu.SetActive(false);
                
                //string inputStr = inputField.text;
                //if(inputStr != null) {
                //    // call draw function
                //    //Debug.Log("my input field get:" + inputStr);
                //    manager.GetComponent<Manager>().calculateInput(inputStr);
                //}
            }
            else {
                inputPanelShow = true;
                inputPanel.SetActive(true);
                objAloneWithAppBar(inputPanel);

                //upSurf.SetActive(true);
                //downSurf.SetActive(true);
                inputMenu.SetActive(true);
                objSuspendInFrontOfCam(inputMenu);



                // TODO: set partical system false
                particlesys.gameObject.SetActive(false);
            }
        }

        private void implicitInpuPanelShowOrHide() {
            if(inputPanelShow) {
                inputPanelShow = false;
                inputPanel.SetActive(false);
                implicitMenu.SetActive(false);
            }
            else {
                inputPanelShow = true;
                inputPanel.SetActive(true);
                implicitMenu.SetActive(true);

                objAloneWithAppBar(inputPanel);
                objSuspendInFrontOfCam(implicitMenu);
                //upSurf.SetActive(false);
                //downSurf.SetActive(false);
                // TODO: play partical system
                particlesys.gameObject.SetActive(true);


            }
        }

        private void pointObserverShowOrHide() {
            manager.GetComponent<Manager>().ChangeObserveMode();
            if(observeModeOn) {
                observeModeOn = false;
                valuePanel.SetActive(false);
            }
            else {
                observeModeOn = true;
                valuePanel.SetActive(true);
                objAloneWithAppBar(valuePanel);
            }
        }

        private void valueAnalysisShowOrHide() {
            if(analysisValueShow) {
                analysisValueShow = false;
                manager.GetComponent<Manager>().hideAnalysisValues();
            }
            else {
                analysisValueShow = true;
                manager.GetComponent<Manager>().showAnalysisValues();
            }
        }

        private void handwritingStart() {
            //photoCanvas.SetActive(true);
            //StartCoroutine(TakePhoto());
            if(inputPanelShow) {
                inputPanelShow = false;
                inputPanel.SetActive(false);
                captureMenu.SetActive(false);
            }
            else {
                photoCanvas.SetActive(true);
                inputPanelShow = true;
                inputPanel.SetActive(true);
                captureMenu.SetActive(true);

                objAloneWithAppBar(inputPanel);
                objSuspendInFrontOfCam(captureMenu);
                StartCoroutine(TakePhoto());


            }
        }

        IEnumerator TakePhoto() {
            yield return new WaitForSeconds(4.0f);
            // call functions to take photo and OCR
            screenCapture.GetComponent<ScreenCapture>().ocrStart();
            photoCanvas.SetActive(false);
        }

        public void objSuspendInFrontOfCam(GameObject obj) {
            obj.transform.position = cameraTransform.position + cameraTransform.forward * 1.0f + cameraTransform.right * 0.26f;
            obj.transform.rotation = cameraTransform.rotation;
        }

        public void objAloneWithAppBar(GameObject obj) {
            obj.transform.position = transform.position + transform.up*0.3f;
            obj.transform.rotation = transform.rotation;
        }
    }
}
