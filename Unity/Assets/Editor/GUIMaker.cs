using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GUIMaker : EditorWindow {

	bool IntRounding = true; //Disable rounding to nearest integer when performing a multi-selection resize.
	bool IncludeHeaders = true; //Disable putting the 'header' bits into the saved script.
	bool ShowHelpDialog = true; //Disable the F1 help screen everytime.
	//You will need to close and re-open the editor window for these changes to take effect.


	#region Editor variables
	//spews alot of info, you probably wont need this.
	bool DebugMode = false;



	static int screenSizeX = 800, screenSizeY = 600;
	int screenSizeXold = 800, screenSizeYold = 600, contextWidth = 150, contextHeight = 205, safety = 0, screenTop = 50, screenLeft = 3;
	string result;
	GUISkin editorSkin, previewSkin;
	Color BGCol = new Color( 0.192f, 0.302f, 0.475f ), contextColor, contextColorOld;
	Vector2 mousePos, screenMousePos, clickOffset, clickStartLocation, tempVector;
	bool mouseOnScreen = false, clickedToMakeNew = false, finishedClickToMakeNew = true, clickedOnExistingElement = false,
		clickedOnOffsetHandle = false, clickedOnScrollHandle = false, clickedToResize = false, pointingAtElement = false,
		resetting = false, showContext = false, mouseOnContext = false, waitingToFocus = false, waitingToFocusDummy = false,
		dragCopy = false, showAllElements = false, snapDisplay = false, snapKB = false, slideDisplay = false, slideKB = false,
		disablePreviews = true, addAsList = false, insertingToGroup = false, slideOnX = false, slideOnY = false, selectingANewGroup = false,
		haveASelectionGroup = false, appendingSelection = false, chooseSaveType = false, inCSharp = true, usesEditors = false,
		groupElementSpecificBool = false, groupElementSpecificBoolOld = false;
	Rect screenRect, contextRect, parentRect, tempRect, resizeStepRect, selectingRect, completeGroupRect;
	List<Vector2> resizeStartRatios;
	int chosenGUIPart = 0, chosenGUIPartOld = 0, contextGUIPart = 0, groupElementSpecficInt1 = 0, groupElementSpecficInt1Old = 0, groupElementSpecficInt2 = 0, groupElementSpecficInt2Old = 0;
	float groupElementSpecficFloat1 = 0, groupElementSpecficFloat1Old = 0, groupElementSpecficFloat2 = 0, groupElementSpecficFloat2Old = 0;
	string focusThisControl = "";
	int selectedElementIndex = -1, newElementIndex = -1, pointedElementIndex = -1, resizeCorner = 0;
	delegate void GUIDelegate ( GUIElement element, Vector2 offset );
	List<GUIElement> allElements;
	List<GUIElement> elementsToDelete, currentSelectedElements;
	GUIElement screenElement;
	enum AnchorLocation { TopLeft = 0, TopCenter = 1, TopRight = 2, MiddleLeft = 3, MiddleCenter = 4, MiddleRight = 5, BottomLeft = 6, BottomCenter = 7, BottomRight = 8 }
	AnchorLocation contextAnchor, contextAnchorOld;
	int[] userVariableCounts;
	List<string> userVariableDeclarations, userVariables, warnings, resultLines, windowDeclarations;
	List<List<string>> allWindows;
	int tabCount, currentEditorLabelSize = 0, listCount = 0;
	bool writingToWindow = false, confirmLoad = false;
	GUIElement writtenWindow;
	Color currentColor;
	string saveCode, loadCode, debugString = "";
	Rect helpWindowRect = new Rect( 50, 60, 520, 570 );

	#region GUI Type lookups
	GUIDelegate[] GUIDelegates;

	string[] typesOfGUIElement = new string[] {
		"Select Multiple",
		"GUI - Label",
		"GUI - Box",
		"GUI - Button",
		"GUI - Repeat Button",
		"GUI - Selection Grid",
		"GUI - Toggle",
		"GUI - DrawTexture",
		"GUI - Text Field",
		"GUI - Text Area",
		"GUI - Slider - Horizontal",
		"GUI - Slider - Vertical",
		"GUI - Scrollbar - Horizontal",
		"GUI - Scrollbar - Vertical",
		"GUI - Group",
		"GUI - Scrollview",
		"GUI - Window",
		"EditorGUI - Shadow Label",
		"EditorGUI - Int Field",
		"EditorGUI - Float Field",
		"EditorGUI - Object Field",
		"EditorGUI - Color Field",
		"EditorGUI - Vector Field",
		"EditorGUI - Rect Field",
		"EditorGUI - Enum Popup",
		"EditorGUI - Foldout",
		"EditorGUI - Window"
	};

	string[] defailtVariableNames = new string[] {
		"",	//	"Screen",
		"",	//	"GUI - Label",
		"",	//	"GUI - Box",
		"",	//	"GUI - Button",
		"",	//	"GUI - Repeat Button",
		"someSelectionGridInteger",	//	"GUI - Selection Grid",
		"someToggleBoolean",	//	"GUI - Toggle",
		"someTexture",	//	"GUI - DrawTexture",
		"someTextFieldebugStringing",	//	"GUI - Text Field",
		"someTextAreaString",	//	"GUI - Text Area",
		"someHSliderFloat",	//	"GUI - Slider - Horizontal",
		"someVSliderFloat",	//	"GUI - Slider - Vertical",
		"someHScrollFloat",	//	"GUI - Scrollbar - Horizontal",
		"someVScrollFloat",	//	"GUI - Scrollbar - Vertical",
		"",	//	"GUI - Group",
		"someScrollviewVector",	//	"GUI - Scrollview",
		"someWindowRect",	//	"GUI - Window",
		"",	//	"EditorGUI - Shadow Label",
		"someEditorInt",	//	"EditorGUI - Int Field",
		"someEditorFloat",	//	"EditorGUI - Float Field",
		"someEditorObject",	//	"EditorGUI - Object Field",
		"someEditorColor",	//	"EditorGUI - Color Field",
		"someEditorVector",	//	"EditorGUI - Vector Field",
		"someEditorRect",	//	"EditorGUI - Rect Field",
		"someEnum",	//	"EditorGUI - Enum Popup",
		"someFoldoutBoolean",	//	"EditorGUI - Foldout",
		"someEWindowRect"	//	"EditorGUI - Window",
	};

	string[] defaultVariableValues = new string[] {
		"",	//	"Screen",
		"",	//	"GUI - Label",
		"",	//	"GUI - Box",
		"",	//	"GUI - Button",
		"",	//	"GUI - Repeat Button",
		"0",	//	"GUI - Selection Grid",
		"false",	//	"GUI - Toggle",
		"someTexture",	//	"GUI - DrawTexture",
		"\"\"",	//	"GUI - Text Field",
		"\"\"",	//	"GUI - Text Area",
		"0",	//	"GUI - Slider - Horizontal",
		"0",	//	"GUI - Slider - Vertical",
		"0",	//	"GUI - Scrollbar - Horizontal",
		"0",	//	"GUI - Scrollbar - Vertical",
		"",	//	"GUI - Group",
		"Vector2.zero",	//	"GUI - Scrollview",
		"new Rect(0,0,0,0)",	//	"GUI - Window",
		"",	//	"EditorGUI - Shadow Label",
		"0",	//	"EditorGUI - Int Field",
		"0",	//	"EditorGUI - Float Field",
		"null",	//	"EditorGUI - Object Field",
		"Color.white",	//	"EditorGUI - Color Field",
		"0",	//	"EditorGUI - Vector Field",
		"new Rect(0,0,0,0)",	//	"EditorGUI - Rect Field",
		"0",	//	"EditorGUI - Enum Popup",
		"false",	//	"EditorGUI - Foldout",
		"new Rect(0,0,0,0)"	//	"EditorGUI - Window",
	};

	string[] defaultLabels = new string[] {
		"",	//	"Screen",
		"Label",	//	"GUI - Label",
		"Box",	//	"GUI - Box",
		"Button",	//	"GUI - Button",
		"Repeat Button",	//	"GUI - Repeat Button",
		"",	//	"GUI - Selection Grid",
		"Toggle",	//	"GUI - Toggle",
		"",	//	"GUI - DrawTexture",
		"",	//	"GUI - Text Field",
		"",	//	"GUI - Text Area",
		"",	//	"GUI - Slider - Horizontal",
		"",	//	"GUI - Slider - Vertical",
		"",	//	"GUI - Scrollbar - Horizontal",
		"",	//	"GUI - Scrollbar - Vertical",
		"",	//	"GUI - Group",
		"",	//	"GUI - Scrollview",
		"Window",	//	"GUI - Window",
		"Label",	//	"EditorGUI - Shadow Label",
		"Int Field",	//	"EditorGUI - Int Field",
		"Float Field",	//	"EditorGUI - Float Field",
		"Object Field",	//	"EditorGUI - Object Field",
		"Color Field",	//	"EditorGUI - Color Field",
		"Vector Field",	//	"EditorGUI - Vector Field",
		"Rect Field",	//	"EditorGUI - Rect Field",
		"Enum Popup",	//	"EditorGUI - Enum Popup",
		"Foldout",	//	"EditorGUI - Foldout",
		"Window"	//	"EditorGUI - Window",
	};

	string[] shortNamesOfGUIElement = new string[] {
		"Screen",
		"Label",
		"Box",
		"Button",
		"R.Button",
		"S.Grid",
		"Toggle",
		"DrawTex",
		"Text Field",
		"Text Area",
		"H Slider",
		"V Slider",
		"H Scrollbar",
		"V Scrollbar",
		"Group",
		"Scrollview",
		"Window",
		"S.Label",
		"Int Field",
		"Float Field",
		"Object Field",
		"Color Field",
		"Vector Field",
		"Rect Field",
		"Enum Popup",
		"Foldout",
		"E.Window"
	};

	bool[] GUIElementsAllowLabel = new bool[] {
		false,	//	"Screen",
		true,	//	"GUI - Label",
		true,	//	"GUI - Box",
		true,	//	"GUI - Button",
		true,	//	"GUI - Repeat Button",
		false,	//	"GUI - Selection Grid",
		true,	//	"GUI - Toggle",
		false,	//	"GUI - DrawTexture",
		false,	//	"GUI - Text Field",
		false,	//	"GUI - Text Area",
		false,	//	"GUI - Slider - Horizontal",
		false,	//	"GUI - Slider - Vertical",
		false,	//	"GUI - Scrollbar - Horizontal",
		false,	//	"GUI - Scrollbar - Vertical",
		false,	//	"GUI - Group",
		false,	//	"GUI - Scrollview",
		true,	//	"GUI - Window",
		true,	//	"EditorGUI - Shadow Label",
		true,	//	"EditorGUI - Int Field",
		true,	//	"EditorGUI - Float Field",
		true,	//	"EditorGUI - Object Field",
		true,	//	"EditorGUI - Color Field",
		true,	//	"EditorGUI - Vector Field",
		true,	//	"EditorGUI - Rect Field",
		true,	//	"EditorGUI - Enum Popup",
		true,	//	"EditorGUI - Foldout",
		true	//	"EditorGUI - Window",
	};

	bool[] GUIElementsCanContainOthers = new bool[] {
		true,	//	"Screen",
		false,	//	"GUI - Label",
		false,	//	"GUI - Box",
		false,	//	"GUI - Button",
		false,	//	"GUI - Repeat Button",
		false,	//	"GUI - Selection Grid",
		true,	//	"GUI - Toggle",
		false,	//	"GUI - DrawTexture",
		false,	//	"GUI - Text Field",
		false,	//	"GUI - Text Area",
		false,	//	"GUI - Slider - Horizontal",
		false,	//	"GUI - Slider - Vertical",
		false,	//	"GUI - Scrollbar - Horizontal",
		false,	//	"GUI - Scrollbar - Vertical",
		true,	//	"GUI - Group",
		true,	//	"GUI - Scrollview",
		true,	//	"GUI - Window",
		false,	//	"EditorGUI - Shadow Label",
		false,	//	"EditorGUI - Int Field",
		false,	//	"EditorGUI - Float Field",
		false,	//	"EditorGUI - Object Field",
		false,	//	"EditorGUI - Color Field",
		false,	//	"EditorGUI - Vector Field",
		false,	//	"EditorGUI - Rect Field",
		false,	//	"EditorGUI - Enum Popup",
		true,	//	"EditorGUI - Foldout",
		true	//	"EditorGUI - Window",
	};

	string[] GUIElementsVariableType = new string[] {
		"None",	//	"Screen",
		"None",	//	"GUI - Label",
		"None",	//	"GUI - Box",
		"None",	//	"GUI - Button",
		"None",	//	"GUI - Repeat Button",
		"Int",	//	"GUI - Selection Grid",
		"Bool",	//	"GUI - Toggle",
		"Texture",	//	"GUI - DrawTexture",
		"String",	//	"GUI - Text Field",
		"String",	//	"GUI - Text Area",
		"Float",	//	"GUI - Slider - Horizontal",
		"Float",	//	"GUI - Slider - Vertical",
		"Float",	//	"GUI - Scrollbar - Horizontal",
		"Float",	//	"GUI - Scrollbar - Vertical",
		"None",	//	"GUI - Group",
		"Vector2",	//	"GUI - Scrollview",
		"Rect",	//	"GUI - Window",
		"None",	//	"EditorGUI - Shadow Label",
		"Int",	//	"EditorGUI - Int Field",
		"Float",	//	"EditorGUI - Float Field",
		"Object",	//	"EditorGUI - Object Field",
		"Color",	//	"EditorGUI - Color Field",
		"Vector",	//	"EditorGUI - Vector Field",
		"Rect",	//	"EditorGUI - Rect Field",
		"Enum",	//	"EditorGUI - Enum Popup",
		"Bool",	//	"EditorGUI - Foldout",
		"Rect"	//	"EditorGUI - Window",
	};

	string[] GUIElementsVariableTypeCased = new string[] {
		"None",	//	"Screen",
		"None",	//	"GUI - Label",
		"None",	//	"GUI - Box",
		"None",	//	"GUI - Button",
		"None",	//	"GUI - Repeat Button",
		"int",	//	"GUI - Selection Grid",
		"bool",	//	"GUI - Toggle",
		"Texture2D",	//	"GUI - DrawTexture",
		"string",	//	"GUI - Text Field",
		"string",	//	"GUI - Text Area",
		"float",	//	"GUI - Slider - Horizontal",
		"float",	//	"GUI - Slider - Vertical",
		"float",	//	"GUI - Scrollbar - Horizontal",
		"float",	//	"GUI - Scrollbar - Vertical",
		"None",	//	"GUI - Group",
		"Vector2",	//	"GUI - Scrollview",
		"Rect",	//	"GUI - Window",
		"None",	//	"EditorGUI - Shadow Label",
		"int",	//	"EditorGUI - Int Field",
		"float",	//	"EditorGUI - Float Field",
		"Object",	//	"EditorGUI - Object Field",
		"Color",	//	"EditorGUI - Color Field",
		"Vector",	//	"EditorGUI - Vector Field",
		"Rect",	//	"EditorGUI - Rect Field",
		"enum",	//	"EditorGUI - Enum Popup",
		"bool",	//	"EditorGUI - Foldout",
		"Rect",	//	"EditorGUI - Window",
		"string[]" // "Selection grid strings
	};

	string[] contextTypesLabels = new string[] {
		"ChangeGUI:",
		"GUI - Label",
		"GUI - Box",
		"GUI - Button",
		"GUI - Repeat Button",
		"GUI - Selection Grid",
		"GUI - Toggle",
		"GUI - DrawTexture",
		"GUI - Text Field",
		"GUI - Text Area",
		"GUI - Slider - Horizontal",
		"GUI - Slider - Vertical",
		"GUI - Scrollbar - Horizontal",
		"GUI - Scrollbar - Vertical",
		"GUI - Group",
		"GUI - Scrollview",
		"GUI - Window",
		"EditorGUI - Shadow Label",
		"EditorGUI - Int Field",
		"EditorGUI - Float Field",
		"EditorGUI - Object Field",
		"EditorGUI - Color Field",
		"EditorGUI - Vector Field",
		"EditorGUI - Rect Field",
		"EditorGUI - Enum Popup",
		"EditorGUI - Foldout",
		"EditorGUI - Window"
	};


	struct TypesOfGUIElement {
		public const int SelectMany = 0;
		public const int GUILabel = 1;
		public const int GUIBox = 2;
		public const int GUIButton = 3;
		public const int GUIRepeatButton = 4;
		public const int GUISelectionGrid = 5;
		public const int GUIToggle = 6;
		public const int GUIDrawTexture = 7;
		public const int GUITextField = 8;
		public const int GUITextArea = 9;
		public const int GUISliderHorizontal = 10;
		public const int GUISliderVertical = 11;
		public const int GUIScrollbarHorizontal = 12;
		public const int GUIScrollbarVertical = 13;
		public const int GUIGroup = 14;
		public const int GUIScrollview = 15;
		public const int GUIWindow = 16;
		public const int EditorGUIShadowLabel = 17;
		public const int EditorGUIIntField = 18;
		public const int EditorGUIFloatField = 19;
		public const int EditorGUIObjectField = 20;
		public const int EditorGUIColorField = 21;
		public const int EditorGUIVectorField = 22;
		public const int EditorGUIRectField = 23;
		public const int EditorGUIEnumPopup = 24;
		public const int EditorGUIFoldout = 25;
		public const int EditorGUIWindow = 26;
	};


	#endregion

	//fake variables
	int someInt = 0;
	float someFloat = 0.0f;
	Rect someRect;
	Vector4 someVector;
	Vector2 someScrollView = Vector2.zero;
	Object someObject = null;
	Color someColor = Color.red;
	enum someEnum { Choices, Selections, Options };

	#endregion



	class GUIElement {
		public GUIDelegate myGUI;
		public int partID, oldID, elementSpecificInt1, elementSpecificInt2;
		public float elementSpecificFloat1, elementSpecificFloat2;
		public AnchorLocation anchoredTo;
		public List<GUIElement> groupedElements;
		public GUIElement containingElement;
		public Rect relativeRect;
		public bool iAmList, elementSpecificBool;
		public Vector2 listOffset, scrollOffset;
		public string label, variableName, myName;
		public Color partColor;
		public GUIElement ( GUIElement original ) {
			this.partID = original.partID;
			this.oldID = original.oldID;
			this.myGUI = original.myGUI;
			this.containingElement = original.containingElement;
			this.relativeRect = original.relativeRect;
			this.iAmList = original.iAmList;
			this.listOffset = original.listOffset;
			this.scrollOffset = original.scrollOffset;
			this.label = original.label;
			this.variableName = original.variableName;
			this.partColor = original.partColor;
			this.myName = original.myName;
			this.anchoredTo = original.anchoredTo;
			this.elementSpecificInt1 = original.elementSpecificInt1;
			this.elementSpecificInt2 = original.elementSpecificInt2;
			this.elementSpecificFloat1 = original.elementSpecificFloat1;
			this.elementSpecificFloat2 = original.elementSpecificFloat2;
			this.elementSpecificBool = original.elementSpecificBool;
		}
		public GUIElement () {
			this.partID = 0;
			this.oldID = 0;
			this.groupedElements = new List<GUIElement>();
			this.iAmList = false;
			this.listOffset = new Vector2( 0, 0 );
			this.scrollOffset = new Vector2( 0, 0 );
			this.partColor = Color.white;
			this.anchoredTo = AnchorLocation.TopLeft;
			this.elementSpecificInt1 = 4;
			this.elementSpecificInt2 = 4;
			this.elementSpecificFloat1 = 0;
			this.elementSpecificFloat2 = 1;
			this.variableName = "";
			this.label = "";
		}
	}


	#region Startup

	[MenuItem( "Window/GUIMaker/GUIMaker" )]
	static void Init () {
		GUIMaker window = (GUIMaker) EditorWindow.GetWindow( typeof( GUIMaker ) );
		window.Show();
		window.position = new Rect( 70, 80, screenSizeX + 6, screenSizeY + 73 );
	}

	void Awake () {
		Reset();
	}

	void Reset () {
		GUIDelegates = new GUIDelegate[27];
		GUIDelegates[0] = GNothing;
		GUIDelegates[1] = GLabel;
		GUIDelegates[2] = GBox;
		GUIDelegates[3] = GButton;
		GUIDelegates[4] = GRepeatButton;
		GUIDelegates[5] = GSelectionGrid;
		GUIDelegates[6] = GToggle;
		GUIDelegates[7] = GDrawTexture;
		GUIDelegates[8] = GTextField;
		GUIDelegates[9] = GTextArea;
		GUIDelegates[10] = GSliderHorizontal;
		GUIDelegates[11] = GSliderVertical;
		GUIDelegates[12] = GScrollbarHorizontal;
		GUIDelegates[13] = GScrollbarVertical;
		GUIDelegates[14] = GGroup;
		GUIDelegates[15] = GScrollview;
		GUIDelegates[16] = GWindow;
		GUIDelegates[17] = GShadowLabel;
		GUIDelegates[18] = GIntField;
		GUIDelegates[19] = GFloatField;
		GUIDelegates[20] = GObjectField;
		GUIDelegates[21] = GColorField;
		GUIDelegates[22] = GVectorField;
		GUIDelegates[23] = GRectField;
		GUIDelegates[24] = GEnumPopup;
		GUIDelegates[25] = GFoldout;
		GUIDelegates[26] = GEditorWindow;

		editorSkin = EditorGUIUtility.Load( "GUIMaker/GUIMakerSkin.guiskin" ) as GUISkin;
		allElements = new List<GUIElement>();
		GUIElement newElement = new GUIElement();
		newElement.myName = "Screen";
		newElement.relativeRect = new Rect( 0, 0, screenSizeX, screenSizeY );
		newElement.containingElement = null;
		newElement.myGUI = GUIDelegates[0];
		allElements.Add( newElement );
		screenElement = newElement;
		pointingAtElement = false;
		selectingRect = new Rect( 0, 0, 0, 0 );
		completeGroupRect = new Rect( 0, 0, 0, 0 );

		selectedElementIndex = -1;
		currentSelectedElements = new List<GUIElement>();
		currentSelectedElements.Add( null );
		resizeStartRatios = new List<Vector2>();
		result = "";
		HideContext();
	}

	#endregion

	void OnGUI () {

		#region Ungrouped
		if ( allElements == null ) Reset();
		debugString = "";
		wantsMouseMove = true;
		screenRect = new Rect( screenLeft, screenTop, screenSizeX, screenSizeY );
		Event e = Event.current;
		mousePos = new Vector2( e.mousePosition.x, e.mousePosition.y );
		mouseOnScreen = screenRect.Contains( mousePos ) && !ShowHelpDialog;
		mouseOnContext = contextRect.Contains( mousePos );
		screenMousePos = mousePos - new Vector2( screenLeft, screenTop );


		if ( waitingToFocus ) {
			if ( waitingToFocusDummy ) {
				waitingToFocusDummy = false;
				waitingToFocus = false;
				GUI.FocusControl( focusThisControl );
			} else {
				waitingToFocusDummy = true;
			}
		}
		#endregion

		#region KeyStrokes

		if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.F1 ) {
			ShowHelpDialog = !ShowHelpDialog;
			this.Repaint();
		}

		if ( GUI.GetNameOfFocusedControl() != "Element Label" && GUI.GetNameOfFocusedControl() != "Element Variable" ) {
			slideKB = e.shift;
			snapKB = e.control;

			if ( e.type == EventType.KeyDown && currentSelectedElements[0] != null && mouseOnScreen ) {
				switch ( e.keyCode ) {
					case KeyCode.Delete:
						DeleteElements();
						break;
					case KeyCode.UpArrow:
						foreach ( GUIElement element in currentSelectedElements ) {
							if ( e.shift ) {
								element.relativeRect.height--;
							} else if ( e.control && element.iAmList ) {
								element.listOffset.y--;
							} else {
								element.relativeRect.y--;
							}
						}
						if ( haveASelectionGroup ) completeGroupRect.y--;
						break;
					case KeyCode.DownArrow:
						foreach ( GUIElement element in currentSelectedElements ) {
							if ( e.shift ) {
								element.relativeRect.height++;
							} else if ( e.control && element.iAmList ) {
								element.listOffset.y++;
							} else {
								element.relativeRect.y++;
							}
						}
						if ( haveASelectionGroup ) completeGroupRect.y++;
						break;
					case KeyCode.LeftArrow:
						foreach ( GUIElement element in currentSelectedElements ) {
							if ( e.shift ) {
								element.relativeRect.width--;
							} else if ( e.control && element.iAmList ) {
								element.listOffset.x--;
							} else {
								element.relativeRect.x--;
							}
						}
						if ( haveASelectionGroup ) completeGroupRect.x--;
						break;
					case KeyCode.RightArrow:
						foreach ( GUIElement element in currentSelectedElements ) {
							if ( e.shift ) {
								element.relativeRect.width++;
							} else if ( e.control && element.iAmList ) {
								element.listOffset.x++;
							} else {
								element.relativeRect.x++;
							}
						}
						if ( haveASelectionGroup ) completeGroupRect.x++;
						break;
				}
				this.Repaint();
			}
		} else {
			if ( e.keyCode == KeyCode.Return ) {
				HideContext();
			}

		}





		#endregion

		#region MouseOver
		if ( e.type == EventType.MouseMove && mouseOnScreen ) {
			pointingAtElement = false;
			for ( pointedElementIndex = allElements.Count - 1; pointedElementIndex > 0; pointedElementIndex-- ) {
				if ( ScreenspaceRect( allElements[pointedElementIndex] ).Contains( screenMousePos ) ) {
					pointingAtElement = true;
					break;
				}
			}
			this.Repaint();

		}
		#endregion

		#region MouseDown
		if ( chosenGUIPart == TypesOfGUIElement.SelectMany && !insertingToGroup ) {
			if ( e.type == EventType.MouseDown && e.button == 0 && mouseOnScreen ) {
				if ( !mouseOnContext ) {
					if ( showContext ) {
						HideContext();
					}

					clickStartLocation = screenMousePos;
					if ( haveASelectionGroup ) {
						if ( completeGroupRect.Contains( screenMousePos ) ) {
							clickedOnExistingElement = true;
							tempRect = completeGroupRect;
							resizeStepRect = tempRect;
							//Test For Resizes
							if ( new Rect( completeGroupRect.xMax - 10, completeGroupRect.yMax - 10, 10, 10 ).Contains( screenMousePos ) ) {
								resizeCorner = 3; //Bottom Right
								clickedOnExistingElement = false;
								clickedToResize = true;
								clickOffset = screenMousePos - new Vector2( completeGroupRect.xMax, completeGroupRect.yMax );
							} else if ( new Rect( completeGroupRect.x, completeGroupRect.yMax - 10, 10, 10 ).Contains( screenMousePos ) ) {
								resizeCorner = 1; //Bottom Left
								clickedOnExistingElement = false;
								clickedToResize = true;
								clickOffset = screenMousePos - new Vector2( completeGroupRect.x, completeGroupRect.yMax );
							} else if ( new Rect( completeGroupRect.xMax - 10, completeGroupRect.y, 10, 10 ).Contains( screenMousePos ) ) {
								resizeCorner = 2; //Top Right
								clickedOnExistingElement = false;
								clickedToResize = true;
								clickOffset = screenMousePos - new Vector2( completeGroupRect.xMax, completeGroupRect.y );
							} else if ( new Rect( completeGroupRect.x, completeGroupRect.y, 10, 10 ).Contains( screenMousePos ) ) {
								resizeCorner = 0; //Top Left
								clickedOnExistingElement = false;
								clickedToResize = true;
								clickOffset = screenMousePos - new Vector2( completeGroupRect.x, completeGroupRect.y );
							} else {
								clickOffset = screenMousePos - new Vector2( completeGroupRect.x, completeGroupRect.y );
								dragCopy = e.control;
							}
							if ( clickedToResize ) {
								resizeStartRatios.Clear();
								for ( int i = 0; i < currentSelectedElements.Count; i++ ) {
									Rect relativePos = ReverseOffsetARect( ScreenspaceRect( currentSelectedElements[i] ), completeGroupRect );
									resizeStartRatios.Add( new Vector2( relativePos.x / completeGroupRect.width, relativePos.y / completeGroupRect.height ) );
								}
							}
						} else {
							if ( e.shift ) {
								appendingSelection = true;
							} else {
								ClearSelection();
							}
							completeGroupRect = new Rect( 0, 0, 0, 0 );
							selectingRect.x = screenMousePos.x;
							selectingRect.y = screenMousePos.y;
							selectingANewGroup = true;
						}

					} else {
						selectingRect.x = screenMousePos.x;
						selectingRect.y = screenMousePos.y;
						selectingANewGroup = true;
					}
				}
			}
		} else {
			selectingANewGroup = false;
			if ( e.type == EventType.MouseDown && e.button == 0 ) {
				if ( !mouseOnContext ) {
					if ( showContext ) {
						HideContext();
					}

					clickStartLocation = screenMousePos;
					if ( !e.alt ) {
						Rect testRect;
						for ( int testingIndex = allElements.Count - 1; testingIndex > -1; testingIndex-- ) {
							if ( currentSelectedElements.Count == 1 && currentSelectedElements[0] != null && allElements[testingIndex].iAmList && testingIndex == selectedElementIndex ) {
								testRect = ScreenspaceRect( allElements[testingIndex] );
								testRect.x += allElements[testingIndex].listOffset.x - 6;
								testRect.y += allElements[testingIndex].listOffset.y - 6;
								testRect.width = testRect.height = 12;
								if ( testRect.Contains( screenMousePos ) ) {
									clickedOnOffsetHandle = true;
									break;
								}
							}
							if ( currentSelectedElements.Count == 1 && currentSelectedElements[0] != null && allElements[testingIndex].partID == TypesOfGUIElement.GUIScrollview && testingIndex == selectedElementIndex ) {
								testRect = ScreenspaceRect( allElements[testingIndex] );
								testRect.x += allElements[testingIndex].scrollOffset.x - 6;
								testRect.y += allElements[testingIndex].scrollOffset.y - 6;
								testRect.width = testRect.height = 12;
								if ( testRect.Contains( screenMousePos ) ) {
									clickedOnScrollHandle = true;
									break;
								}
							}
							testRect = ScreenspaceRect( allElements[testingIndex] );
							if ( testRect.Contains( screenMousePos ) ) {
								if ( insertingToGroup ) {
									ReGroupElements( testingIndex );
								} else {
									if ( testingIndex != 0 ) {
										clickedOnExistingElement = true;
										selectedElementIndex = testingIndex;
										currentSelectedElements[0] = allElements[testingIndex];
									}
								}
								break;
							}
						}
					}
					if ( clickedOnExistingElement ) { //Clicked on an existing element
						tempRect = currentSelectedElements[0].relativeRect;
						resizeStepRect = tempRect;
						//Test For Resizes
						Rect myAbsoulteRect = ScreenspaceRect( currentSelectedElements[0] );
						if ( new Rect( myAbsoulteRect.xMax - 9, myAbsoulteRect.yMax - 9, 9, 9 ).Contains( screenMousePos ) ) {
							resizeCorner = 3; //Bottom Right
							clickedOnExistingElement = false;
							clickedToResize = true;
							clickOffset = screenMousePos - new Vector2( currentSelectedElements[0].relativeRect.xMax, currentSelectedElements[0].relativeRect.yMax );
						} else if ( new Rect( myAbsoulteRect.x, myAbsoulteRect.yMax - 9, 9, 9 ).Contains( screenMousePos ) ) {
							resizeCorner = 1; //Bottom Left
							clickedOnExistingElement = false;
							clickedToResize = true;
							clickOffset = screenMousePos - new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.yMax );
						} else if ( new Rect( myAbsoulteRect.xMax - 9, myAbsoulteRect.y, 9, 9 ).Contains( screenMousePos ) ) {
							resizeCorner = 2; //Top Right
							clickedOnExistingElement = false;
							clickedToResize = true;
							clickOffset = screenMousePos - new Vector2( currentSelectedElements[0].relativeRect.xMax, currentSelectedElements[0].relativeRect.y );
						} else if ( new Rect( myAbsoulteRect.x, myAbsoulteRect.y, 9, 9 ).Contains( screenMousePos ) ) {
							resizeCorner = 0; //Top Left
							clickedOnExistingElement = false;
							clickedToResize = true;
							clickOffset = screenMousePos - new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.y );
						} else {
							clickOffset = screenMousePos - new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.y );
							dragCopy = e.control;
						}

					} else if ( clickedOnOffsetHandle ) {
						clickOffset = new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.y ) + currentSelectedElements[0].listOffset - screenMousePos;
						tempVector = currentSelectedElements[0].listOffset;
					} else if ( clickedOnScrollHandle ) {
						clickOffset = new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.y ) + currentSelectedElements[0].scrollOffset - screenMousePos;
						tempVector = currentSelectedElements[0].scrollOffset;
					} else { //Clicked to make a new element
						if ( chosenGUIPart != TypesOfGUIElement.SelectMany && mouseOnScreen ) {
							clickedToMakeNew = true;
							finishedClickToMakeNew = false;
						}
					}
					this.Repaint();
				}
			}
		}

		#endregion

		#region MouseDrag
		//Mouse Drag for new elements
		if ( e.type == EventType.MouseDrag && e.button == 0 && mouseOnScreen ) {
			if ( slideKB || slideDisplay ) {
				Vector2 dragOffset = clickStartLocation - screenMousePos;
				if ( Mathf.Abs( dragOffset.x ) > Mathf.Abs( dragOffset.y ) ) {
					slideOnX = true;
					slideOnY = false;
				} else {
					slideOnY = true;
					slideOnX = false;
				}
			} else {
				slideOnX = false;
				slideOnY = false;
			}

			if ( clickedToMakeNew ) {
				if ( !finishedClickToMakeNew ) { //To add a new element
					finishedClickToMakeNew = true;
					GUIElement newElement = new GUIElement();
					newElement.partID = newElement.oldID = chosenGUIPart;
					if ( chosenGUIPart == TypesOfGUIElement.EditorGUIFoldout || chosenGUIPart == TypesOfGUIElement.GUIToggle ) newElement.elementSpecificBool = true;
					newElement.myGUI = GUIDelegates[newElement.partID];
					newElement.myName = shortNamesOfGUIElement[newElement.partID];
					newElement.label = defaultLabels[newElement.partID];
					if ( addAsList ) {
						addAsList = false;
						newElement.iAmList = true;
					}
					if ( e.alt && currentSelectedElements.Count == 1 && currentSelectedElements[0] != null && (
						currentSelectedElements[0].partID == TypesOfGUIElement.GUIGroup ||
						currentSelectedElements[0].partID == TypesOfGUIElement.GUIWindow ||
						currentSelectedElements[0].partID == TypesOfGUIElement.GUIToggle ||
						currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIFoldout ||
						currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIWindow ||
						currentSelectedElements[0].partID == TypesOfGUIElement.GUIScrollview ) ) {
						currentSelectedElements[0].groupedElements.Add( newElement );
						newElement.containingElement = currentSelectedElements[0];
						parentRect = ScreenspaceRect( FirstNonToggle( currentSelectedElements[0] ) );
						newElement.relativeRect = new Rect( screenMousePos.x - parentRect.x, screenMousePos.y - parentRect.y, 8, 8 );
					} else {
						screenElement.groupedElements.Add( newElement );
						newElement.containingElement = screenElement;
						newElement.relativeRect = new Rect( screenMousePos.x, screenMousePos.y, 8, 8 );
					}
					ReassesAllElements();
					newElementIndex = allElements.IndexOf( newElement );

					if ( !e.alt ) {
						selectedElementIndex = newElementIndex;
						currentSelectedElements[0] = newElement;

					}
				}

				Vector2 snappedPos = SnapPos( screenMousePos, selectedElementIndex );
				if ( allElements[newElementIndex].containingElement != screenElement ) {
					parentRect = ScreenspaceRect( FirstNonToggle( allElements[newElementIndex].containingElement ) );
					allElements[newElementIndex].relativeRect.xMax = snappedPos.x - parentRect.x;
					allElements[newElementIndex].relativeRect.yMax = snappedPos.y - parentRect.y;
				} else {
					allElements[newElementIndex].relativeRect.xMax = snappedPos.x;
					allElements[newElementIndex].relativeRect.yMax = snappedPos.y;
				}
				if ( allElements[newElementIndex].iAmList ) {
					allElements[newElementIndex].listOffset.y = allElements[newElementIndex].relativeRect.height;
				}
				if ( allElements[newElementIndex].partID == TypesOfGUIElement.GUIScrollview ) {
					allElements[newElementIndex].scrollOffset.x = (int) allElements[newElementIndex].relativeRect.width - 10;
					allElements[newElementIndex].scrollOffset.y = (int) allElements[newElementIndex].relativeRect.height - 10;
				}
				allElements[newElementIndex].relativeRect.width = Mathf.Max( allElements[newElementIndex].relativeRect.width, 8 );
				allElements[newElementIndex].relativeRect.height = Mathf.Max( allElements[newElementIndex].relativeRect.height, 8 );
				this.Repaint();
			}

			//Mouse Drag for move existing elements
			if ( clickedOnExistingElement ) {
				if ( dragCopy ) {
					dragCopy = false;
					CopyElements( false );
				}
				if ( haveASelectionGroup ) {
					completeGroupRect.x = slideOnY ? tempRect.x : screenMousePos.x - clickOffset.x;
					completeGroupRect.y = slideOnX ? tempRect.y : screenMousePos.y - clickOffset.y;
					if ( snapKB || snapDisplay ) completeGroupRect = SnapGroup();
					foreach ( GUIElement element in currentSelectedElements ) {
						if ( currentSelectedElements.Contains( element.containingElement ) ) {
							if ( element.containingElement.partID != TypesOfGUIElement.GUIToggle && element.containingElement.partID != TypesOfGUIElement.EditorGUIFoldout ) continue;
						}
						element.relativeRect = OffsetARect( element.relativeRect, completeGroupRect, resizeStepRect );
					}
					resizeStepRect = completeGroupRect;
				} else {
					currentSelectedElements[0].relativeRect.x = slideOnY ? tempRect.x : screenMousePos.x - clickOffset.x;
					currentSelectedElements[0].relativeRect.y = slideOnX ? tempRect.y : screenMousePos.y - clickOffset.y;
					if ( snapKB || snapDisplay ) currentSelectedElements[0].relativeRect = SnapRect( currentSelectedElements[0], selectedElementIndex );
				}
			}


			//Mouse Drag for Resize existing elements
			if ( clickedToResize ) {
				if ( haveASelectionGroup ) { //Resize selection group
					Vector2 snappedPos = SnapGroupPos( screenMousePos - clickOffset );
					switch ( resizeCorner ) {
						case 0: //Top left corner
							completeGroupRect = Rect.MinMaxRect(
								slideOnY ? tempRect.x : snappedPos.x,
								slideOnX ? tempRect.y : snappedPos.y,
								tempRect.xMax,
								tempRect.yMax );
							break;
						case 1: //Bottom left corner
							completeGroupRect = Rect.MinMaxRect(
								slideOnY ? tempRect.x : snappedPos.x,
								tempRect.y,
								tempRect.xMax,
								slideOnX ? tempRect.yMax : snappedPos.y );
							break;
						case 2: //Top Right corner
							completeGroupRect = Rect.MinMaxRect(
								tempRect.x,
								slideOnX ? tempRect.y : snappedPos.y,
								slideOnY ? tempRect.xMax : snappedPos.x,
								tempRect.yMax );
							break;
						case 3: //Bottom Right corner
							completeGroupRect = Rect.MinMaxRect(
								tempRect.x,
								tempRect.y,
								slideOnY ? tempRect.xMax : snappedPos.x,
								slideOnX ? tempRect.yMax : snappedPos.y );
							break;
					}

					completeGroupRect.width = Mathf.Max( completeGroupRect.width, 8 );
					completeGroupRect.height = Mathf.Max( completeGroupRect.height, 8 );

					Vector2 scaleFactor = new Vector2( resizeStepRect.width / completeGroupRect.width, resizeStepRect.height / completeGroupRect.height );

					for ( int i = 0; i < currentSelectedElements.Count; i++ ) {
						currentSelectedElements[i].relativeRect.width /= scaleFactor.x;
						currentSelectedElements[i].relativeRect.height /= scaleFactor.y;

						Rect meInGroupRect = new Rect( completeGroupRect.x + completeGroupRect.width * resizeStartRatios[i].x, completeGroupRect.y + completeGroupRect.height * resizeStartRatios[i].y, currentSelectedElements[i].relativeRect.width, currentSelectedElements[i].relativeRect.height );

						currentSelectedElements[i].relativeRect = RelativeRectFromScreenRect( currentSelectedElements[i], meInGroupRect );
					}
					resizeStepRect = completeGroupRect;

				} else { //resize single
					Vector2 snappedPos = SnapPos( screenMousePos - clickOffset, selectedElementIndex );
					switch ( resizeCorner ) {
						case 0: //Top left corner
							currentSelectedElements[0].relativeRect = Rect.MinMaxRect(
								slideOnY ? tempRect.x : snappedPos.x,
								slideOnX ? tempRect.y : snappedPos.y,
								tempRect.xMax,
								tempRect.yMax );
							break;
						case 1: //Bottom left corner
							currentSelectedElements[0].relativeRect = Rect.MinMaxRect(
								slideOnY ? tempRect.x : snappedPos.x,
								tempRect.y,
								tempRect.xMax,
								slideOnX ? tempRect.yMax : snappedPos.y );
							break;
						case 2: //Top Right corner
							currentSelectedElements[0].relativeRect = Rect.MinMaxRect(
								tempRect.x,
								slideOnX ? tempRect.y : snappedPos.y,
								slideOnY ? tempRect.xMax : snappedPos.x,
								tempRect.yMax );
							break;
						case 3: //Bottom Right corner
							currentSelectedElements[0].relativeRect = Rect.MinMaxRect(
								tempRect.x,
								tempRect.y,
								slideOnY ? tempRect.xMax : snappedPos.x,
								slideOnX ? tempRect.yMax : snappedPos.y );
							break;
					}
					foreach ( GUIElement element in currentSelectedElements[0].groupedElements ) {
						int dx = (int) element.anchoredTo % 3;
						int dy = (int) element.anchoredTo / 3;
						element.relativeRect.x += ( currentSelectedElements[0].relativeRect.width - resizeStepRect.width ) * ( dx / 2f );
						element.relativeRect.y += ( currentSelectedElements[0].relativeRect.height - resizeStepRect.height ) * ( dy / 2f );
					}
					resizeStepRect = currentSelectedElements[0].relativeRect;
					currentSelectedElements[0].relativeRect.width = Mathf.Max( currentSelectedElements[0].relativeRect.width, 8 );
					currentSelectedElements[0].relativeRect.height = Mathf.Max( currentSelectedElements[0].relativeRect.height, 8 );
				}
			}

			//Mouse Drag for move list offset
			if ( clickedOnOffsetHandle ) {
				currentSelectedElements[0].listOffset = SnapPos( screenMousePos + clickOffset, -1 ) - new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.y );
				if ( slideOnY ) currentSelectedElements[0].listOffset.x = tempVector.x;
				if ( slideOnX ) currentSelectedElements[0].listOffset.y = tempVector.y;
			}

			//Mouse Drag for move Scroll offset
			if ( clickedOnScrollHandle ) {
				currentSelectedElements[0].scrollOffset = SnapPos( screenMousePos + clickOffset, -1 ) - new Vector2( currentSelectedElements[0].relativeRect.x, currentSelectedElements[0].relativeRect.y );
				if ( slideOnY ) currentSelectedElements[0].scrollOffset.x = tempVector.x;
				if ( slideOnX ) currentSelectedElements[0].scrollOffset.y = tempVector.y;
			}

			//Mouse Drag for make selection area
			if ( selectingANewGroup ) {
				selectingRect.xMax = screenMousePos.x;
				selectingRect.yMax = screenMousePos.y;
				CollectSelection();
			}
			this.Repaint();
		}
		#endregion

		#region MouseUp
		//Any Mouse Up 
		if ( e.type == EventType.MouseUp && e.button == 0 ) {
			if ( clickedOnExistingElement || clickedToMakeNew || clickedToResize || clickedOnOffsetHandle || clickedOnScrollHandle || selectingANewGroup ) {
				if ( clickedToMakeNew && !finishedClickToMakeNew ) {
					selectedElementIndex = -1;
					currentSelectedElements[0] = null;
				}
				if ( selectingANewGroup ) {
					selectingRect = new Rect( 0, 0, 0, 0 );
					if ( currentSelectedElements[0] != null ) {
						if ( appendingSelection ) {
							for ( int testingIndex = allElements.Count - 1; testingIndex > -1; testingIndex-- ) {
								if ( currentSelectedElements.Contains( allElements[testingIndex] ) ) continue;
								Rect testRect = ScreenspaceRect( allElements[testingIndex] );
								if ( testRect.Contains( screenMousePos ) && testingIndex != 0 ) {
									currentSelectedElements.Add( allElements[testingIndex] );
									completeGroupRect = GrowSelection();
									break;
								}
							}
						}
						haveASelectionGroup = true;
						completeGroupRect = GrowSelection();
					} else {
						for ( int testingIndex = allElements.Count - 1; testingIndex > -1; testingIndex-- ) {
							Rect testRect = ScreenspaceRect( allElements[testingIndex] );
							if ( testRect.Contains( screenMousePos ) && testingIndex != 0 ) {
								clickedOnExistingElement = true;
								selectedElementIndex = testingIndex;
								currentSelectedElements[0] = allElements[testingIndex];
								completeGroupRect = ScreenspaceRect( currentSelectedElements[0] );
								haveASelectionGroup = true;
								completeGroupRect = GrowSelection();
								break;
							}
						}
						if ( !clickedOnExistingElement ) haveASelectionGroup = false;
					}
					selectingANewGroup = false;
					appendingSelection = false;
				}
				if ( currentSelectedElements.Count > 1 ) {
					if ( clickedToResize ) {
						foreach ( GUIElement element in currentSelectedElements ) {
							RoundRect( element );
						}
					}
				}
				clickedToMakeNew = false;
				clickedOnExistingElement = false;
				clickedOnOffsetHandle = false;
				clickedOnScrollHandle = false;
				clickedToResize = false;
				this.Repaint();
			}
		}

		#endregion

		#region Rightclicks

		if ( e.type == EventType.MouseDown && e.button == 1 && mouseOnScreen ) {
			bool goodClick = false;
			if ( chosenGUIPart == TypesOfGUIElement.SelectMany ) {
				if ( haveASelectionGroup && completeGroupRect.Contains( screenMousePos ) ) {
					goodClick = true;
				} else {
					for ( int testingIndex = allElements.Count - 1; testingIndex > 0; testingIndex-- ) {
						if ( ScreenspaceRect( allElements[testingIndex] ).Contains( screenMousePos ) ) {
							currentSelectedElements.Clear();
							currentSelectedElements.Add( allElements[testingIndex] );
							completeGroupRect = GrowSelection();
							selectedElementIndex = testingIndex;
							goodClick = true;
							break;
						}
					}
				}
			} else {
				for ( int testingIndex = allElements.Count - 1; testingIndex > 0; testingIndex-- ) {
					if ( ScreenspaceRect( allElements[testingIndex] ).Contains( screenMousePos ) ) {
						selectedElementIndex = testingIndex;
						currentSelectedElements[0] = allElements[testingIndex];
						goodClick = true;
						break;
					}
				}
			}
			if ( goodClick ) {
				showContext = true;
				contextGUIPart = 0;
				contextColor = contextColorOld = currentSelectedElements[0].partColor;
				contextAnchor = contextAnchorOld = currentSelectedElements[0].anchoredTo;
				GUI.FocusControl( "Dummy" );
				if ( GUIElementsAllowLabel[currentSelectedElements[0].partID] ) {
					ReFocusGUI( "Element Label" );
				}
				contextRect = new Rect( Mathf.Clamp( mousePos.x, screenLeft, screenLeft + screenSizeX - contextWidth ), Mathf.Clamp( mousePos.y, screenTop, screenTop + screenSizeY - contextHeight ), contextWidth, contextHeight );
				this.Repaint();
			}
		}

		#endregion

		#region Draw design GUI
		//This draws the designed gui
		GUI.BeginGroup( new Rect( screenLeft, screenTop, screenSizeX, screenSizeY ) ); //Starts the screen
		GUI.color = BGCol;
		GUI.skin = editorSkin;
		GUI.Box( new Rect( 0, 0, screenSizeX, screenSizeY ), "", "PlainBox" );



		DrawGUIElement( screenElement );

		GUI.enabled = true;
		GUI.color = Color.white;
		GUI.skin = editorSkin;
		if ( selectingANewGroup ) {
			GUI.color = new Color( 1, 1, 1, 0.5f );
			GUI.Box( selectingRect, "", "PlainBox" );
			GUI.color = Color.white;
		} else if ( haveASelectionGroup && !insertingToGroup ) {
			GUI.Box( completeGroupRect, "", "PlainBoxTint" );
			GUI.Box( new Rect( completeGroupRect.x - 2, completeGroupRect.y - 2, 12, 12 ), "", "Handle" );
			GUI.Box( new Rect( completeGroupRect.x - 2, completeGroupRect.yMax - 10, 12, 12 ), "", "Handle" );
			GUI.Box( new Rect( completeGroupRect.xMax - 10, completeGroupRect.y - 2, 12, 12 ), "", "Handle" );
			GUI.Box( new Rect( completeGroupRect.xMax - 10, completeGroupRect.yMax - 10, 12, 12 ), "", "Handle" );
		} else if ( pointingAtElement ) {
			Rect myScreenspaceRect = ScreenspaceRect( allElements[pointedElementIndex] );
			GUI.Box( new Rect( myScreenspaceRect.x - 3, myScreenspaceRect.y - 3, 12, 12 ), "", "Handle" );
			GUI.Box( new Rect( myScreenspaceRect.x - 3, myScreenspaceRect.yMax - 9, 12, 12 ), "", "Handle" );
			GUI.Box( new Rect( myScreenspaceRect.xMax - 9, myScreenspaceRect.y - 3, 12, 12 ), "", "Handle" );
			GUI.Box( new Rect( myScreenspaceRect.xMax - 9, myScreenspaceRect.yMax - 9, 12, 12 ), "", "Handle" );
		}



		GUI.EndGroup(); // Ends the screen

		if ( showAllElements ) {
			GUI.color = Color.Lerp( Color.white, BGCol, 0.5f );
			for ( int i = 1; i < allElements.Count; i++ ) {
				GUI.Box( AbsoluteRect( allElements[i] ), "", "PlainBoxTint" );
			}
		}
		GUI.skin = null;
		#endregion

		#region Editor controls
		//These are the controls for the editor
		GUI.color = Color.white;


		if ( resetting ) {
			if ( GUI.Button( new Rect( 76, 3, 70, 28 ), "Reset!" ) ) {
				Reset();
				resetting = false;
			}
			if ( GUI.Button( new Rect( 76, 33, 70, 15 ), "Cancel" ) ) {
				resetting = false;
			}
		} else {
			if ( GUI.Button( new Rect( 76, 3, 70, 45 ), "Reset" ) ) {
				resetting = true;
			}
		}


		GUI.Label( new Rect( 155, 7, 100, 20 ), "Element:" );
		GUI.color = chosenGUIPart == 0 ? Color.red : Color.white;
		GUI.SetNextControlName( "Part Choice" );
		chosenGUIPart = EditorGUI.Popup( new Rect( 155, 27, 135, 20 ), chosenGUIPart, typesOfGUIElement );
		if ( chosenGUIPart != chosenGUIPartOld ) {
			if ( chosenGUIPartOld == TypesOfGUIElement.SelectMany || chosenGUIPart == TypesOfGUIElement.SelectMany ) {
				haveASelectionGroup = false;
				ClearSelection();
			}
			chosenGUIPartOld = chosenGUIPart;
		}

		GUI.color = addAsList ? Color.white : new Color( 1, 1, 1, 0.4f );
		addAsList = GUI.Toggle( new Rect( 250, 6, 40, 16 ), addAsList, "List", "Button" );
		GUI.color = Color.white;

		snapDisplay = GUI.Toggle( new Rect( 300, 3, 60, 20 ), snapDisplay, "Snap", "Button" );
		slideDisplay = GUI.Toggle( new Rect( 300, 25, 60, 20 ), slideDisplay, "Slide", "Button" );

		if ( snapKB ) GUI.Toggle( new Rect( 300, 3, 60, 20 ), snapKB, "Snap", "Button" );
		if ( slideKB ) GUI.Toggle( new Rect( 300, 25, 60, 20 ), slideKB, "Slide", "Button" );

#if UNITY_4_3
		EditorGUIUtility.labelWidth = 90;
#else
		EditorGUIUtility.LookLikeControls( 90 );
#endif
		GUI.SetNextControlName( "Size X" );
		screenSizeX = EditorGUI.IntField( new Rect( 368, 3, 125, 20 ), "Screen Width", screenSizeX );
		GUI.SetNextControlName( "Size Y" );
		screenSizeY = EditorGUI.IntField( new Rect( 368, 23, 125, 20 ), "Screen Height", screenSizeY );
		if ( screenSizeX != screenSizeXold ) {
			screenSizeX = Mathf.Max( screenSizeX, 8 );
			screenElement.relativeRect.width = screenSizeX;
			foreach ( GUIElement element in allElements ) {
				if ( element.containingElement != screenElement ) continue;
				int temp = (int) element.anchoredTo % 3;
				element.relativeRect.x += ( screenSizeX - screenSizeXold ) * ( temp / 2f );
			}
			position = new Rect( position.x, position.y - 5, Mathf.Max( screenSizeX + 6, 806 ), Mathf.Max( screenSizeY + 73, 120 ) );
			screenSizeXold = screenSizeX;
		}
		if ( screenSizeY != screenSizeYold ) {
			screenSizeY = Mathf.Max( screenSizeY, 8 );
			screenElement.relativeRect.height = screenSizeY;
			foreach ( GUIElement element in allElements ) {
				if ( element.containingElement != screenElement ) continue;
				int temp = (int) element.anchoredTo / 3;
				element.relativeRect.y += ( screenSizeY - screenSizeYold ) * ( temp / 2f );
			}
			position = new Rect( position.x, position.y - 5, Mathf.Max( screenSizeX + 6, 806 ), Mathf.Max( screenSizeY + 73, 120 ) );
			screenSizeYold = screenSizeY;
		}

		GUI.SetNextControlName( "Window Color" );
		BGCol = EditorGUI.ColorField( new Rect( 500, 3, 150, 20 ), "Screen Color", BGCol );
		showAllElements = GUI.Toggle( new Rect( 500, 26, 150, 20 ), showAllElements, "Highlight all Elements" );

#if UNITY_4_3
		EditorGUIUtility.labelWidth = 60;
#else
		EditorGUIUtility.LookLikeControls( 60 );
#endif
		GUI.SetNextControlName( "Preview Skin" );
		previewSkin = (GUISkin) EditorGUI.ObjectField( new Rect( 660, 3, 150, 20 ), "Use skin:", previewSkin, typeof( GUISkin ), false );
#if UNITY_4_3
		EditorGUIUtility.labelWidth = 90;
#else
		EditorGUIUtility.LookLikeControls( 90 );
#endif

		disablePreviews = GUI.Toggle( new Rect( 660, 26, 150, 20 ), disablePreviews, "Disable Elements" );

		if ( currentSelectedElements[0] != null ) {
			if ( currentSelectedElements.Count == 1 ) {
				GUI.Label( new Rect( screenLeft, screenTop + screenSizeY + 3, 800, 20 ), "" + DisplayRect( currentSelectedElements[0] ) + ( currentSelectedElements[0].iAmList ? "  List-based offset: " + currentSelectedElements[0].listOffset : "" ) );
				GUI.skin.button.fontSize = 9;
				GUI.color = Color.Lerp( Color.white, BGCol, 0.5f );
				GUI.Box( new Rect( screenLeft + screenSizeX - 70, screenTop + screenSizeY + 3, 70, 18 ), currentSelectedElements[0].myName );


				GUIElement parent = currentSelectedElements[0].containingElement;
				int count = 1;
				while ( parent != screenElement & count < 8 ) {
					GUI.color = new Color( 1, 1, 1, ( 8 - count ) / 8f );
					if ( GUI.Button( new Rect( screenLeft + screenSizeX - 75 - ( 72 * count ), screenTop + screenSizeY + 3, 70, 18 ), parent.myName + " <" ) ) {
						selectedElementIndex = allElements.IndexOf( parent );
						currentSelectedElements[0] = allElements[selectedElementIndex];

					}
					parent = parent.containingElement;
					count++;
				}
				GUI.skin.button.fontSize = 0;
			} else {
				GUI.Label( new Rect( screenLeft, screenTop + screenSizeY + 3, 400, 20 ), "Currently selecting " + currentSelectedElements.Count + " elements" );
			}
		}
		GUI.color = Color.white;


		BeginWindows();

		if ( showContext && currentSelectedElements[0] != null ) {
			contextRect = GUI.Window( 0, contextRect, ContextMenuFunction, "" );
		}

		if ( ShowHelpDialog ) {
			helpWindowRect = GUI.Window( 0, helpWindowRect, HelpMenuFunction, "F1: Help!" );
		}
		EndWindows();


		#endregion

		#region Save and load
		//This parses the gui into a string, copies it to clipboard
		if ( chooseSaveType ) {
			if ( GUI.Button( new Rect( 3, 3, 34, 28 ), "C#" ) ) {
				chooseSaveType = false;
				inCSharp = true;
				DoSave();
			}
			if ( GUI.Button( new Rect( 39, 3, 34, 28 ), "Js" ) ) {
				chooseSaveType = false;
				inCSharp = false;
				DoSave();
			}
		} else {
			if ( GUI.Button( new Rect( 3, 3, 70, 28 ), "Save" ) ) {
				chooseSaveType = true;
			}
		}
		if ( confirmLoad ) {
			if ( GUI.Button( new Rect( 3, 33, 34, 15 ), "Yes" ) ) {
				confirmLoad = false;
				Reset();
				try {
					ParseLoadCode( EditorGUIUtility.systemCopyBuffer );
				} catch {
					Debug.Log( "Something bad happened in load, code was not good" );
					Reset();
				}
			}
			if ( GUI.Button( new Rect( 39, 33, 34, 15 ), "No" ) ) {
				confirmLoad = false;
			}
		} else {
			if ( GUI.Button( new Rect( 3, 33, 70, 15 ), "Load" ) ) {
				confirmLoad = true;
			}

		}

		#endregion

		#region Debugs
		debugString += "group select: " + selectingANewGroup + "\n";
		debugString += "kb focus is: " + GUI.GetNameOfFocusedControl() + " \n";
		if ( currentSelectedElements[0] != null ) debugString += "Selected count: " + currentSelectedElements.Count + " first of which is " + currentSelectedElements[0].relativeRect + " \n";
		debugString += "Mouse is " + ( mouseOnContext ? "On " : " NOT on" ) + " Context menu \n";

		if ( DebugMode ) {
			GUI.enabled = false;
			GUI.TextArea( new Rect( 50, 300, 700, 400 ), result );
			GUI.enabled = true;

			if ( GUI.Button( new Rect( screenLeft + 300, screenTop + screenSizeY + 3, 110, 19 ), "Log all elements" ) ) {
				for ( int i = 0; i < allElements.Count; i++ ) {
					Debug.Log( allElements[i].myName + " has " + allElements[i].groupedElements.Count + " childs" );
				}
			}

			//GUILayout.BeginArea( new Rect( 100, 100, 500, 500 ) );
			//GUILayout.Label( "mouseOnScreen " + mouseOnScreen );
			//GUILayout.Label( "clickedToMakeNew " + clickedToMakeNew );
			//GUILayout.Label( "clickedToResize " + clickedToResize );
			//GUILayout.Label( "clickedOnOffsetHandle " + clickedOnOffsetHandle );
			//GUILayout.Label( "clickedOnExistingElement " + clickedOnExistingElement );
			//GUILayout.EndArea();

			GUI.Label( new Rect( screenLeft + 5, screenTop + 5, 1000, 1000 ), debugString );
		}

		GUI.SetNextControlName( "Dummy" );
		GUI.Label( new Rect( 0, 0, 0, 0 ), "" );

		#endregion

	}

	#region All the text switches

	void RecursiveComplete ( GUIElement element ) {

		if ( element.partID >= typesOfGUIElement.Length ) {
			Debug.Log( "Element " + element.label + " has a bad ID" );
			return;
		}

		if ( element.partID > 16 ) usesEditors = true;

		string myVariableName = "";
		List<string> theseLines = new List<string>();
		int sizeOfThisElement = 0;


		if ( defailtVariableNames[element.partID] != "" ) {
			if ( element.variableName == "" ) {
				myVariableName = defailtVariableNames[element.partID] + userVariableCounts[element.partID];
				userVariableCounts[element.partID]++;
			} else {
				myVariableName = element.variableName;
			}
			if ( element.partID == TypesOfGUIElement.EditorGUIObjectField ) userVariableDeclarations.Add( "// Recommend: Change Object to the type of object that you need, eg. GameObject, Material\n" );

			if ( element.partID == TypesOfGUIElement.EditorGUIVectorField ) {
				if ( inCSharp ) {
					userVariableDeclarations.Add( GUIElementsVariableTypeCased[element.partID] + element.elementSpecificInt1 + " " + myVariableName + ";\n" );
				} else {
					userVariableDeclarations.Add( "var " + myVariableName + " : " + GUIElementsVariableTypeCased[element.partID] + element.elementSpecificInt1 + ";\n" );
				}

			} else if ( element.partID == TypesOfGUIElement.EditorGUIEnumPopup ) {
				userVariableDeclarations.Add( "enum " + myVariableName + "Choices { One, Two, Three };\n" );
				if ( inCSharp ) {
					userVariableDeclarations.Add( myVariableName + "Choices " + myVariableName + ";\n" );
				} else {
					userVariableDeclarations.Add( "var " + myVariableName + " : " + myVariableName + "Choices;\n" );
				}

			} else if ( element.partID == TypesOfGUIElement.GUIWindow || element.partID == TypesOfGUIElement.EditorGUIWindow ) {
				if ( !writingToWindow ) {
					if ( inCSharp ) {
						userVariableDeclarations.Add( "Rect " + myVariableName + " = new Rect( " + StringifyRect( element ) + ");\n" );
					} else {
						userVariableDeclarations.Add( "var " + myVariableName + ": Rect = Rect( " + StringifyRect( element ) + ");\n" );
					}
				}
			} else {
				if ( inCSharp ) {
					userVariableDeclarations.Add( GUIElementsVariableTypeCased[element.partID] + " " + myVariableName + " = " + defaultVariableValues[element.partID] + ";\n" );
				} else {
					userVariableDeclarations.Add( "var " + myVariableName + " : " + GUIElementsVariableTypeCased[element.partID] + " = " + defaultVariableValues[element.partID] + ";\n" );
				}
			}
			if ( userVariables.Contains( myVariableName ) ) {
				warnings.Add( "//There is more than 1 variable called " + myVariableName + "\n" );
			}
			userVariables.Add( myVariableName );
		}






		if ( element.iAmList ) {
			listCount++;

			userVariableDeclarations.Add( "//someList" + listCount + " is used in a loop somewhere; populate it, or replace 'someList" + listCount + "' with your own collection (and correct Type)\n" );
			if ( inCSharp ) {
				userVariableDeclarations.Add( "List<Object> someList" + listCount + " = new List<Object>();\n" );
			} else {
				userVariableDeclarations.Add( "var someList" + listCount + " :  List.<Object> = new List.<Object>();\n" );
			}

			theseLines.Add( "//someList" + listCount + " needs to be an existing collection eg. List<type> or type[], You may need to change .Count to .Length (for []'s)" );
			theseLines.Add( "for ( " + ( inCSharp ? "int" : "var" ) + " list" + listCount + "Index = 0; list" + listCount + "Index < someList" + listCount + ".Count; list" + listCount + "Index++ ) {" );


			for ( int j = 0; j < theseLines.Count; j++ ) {
				string line = theseLines[j];
				line += "\n";
				for ( int i = 0; i < tabCount; i++ ) {
					line = "\t" + line;
				}

				if ( writingToWindow ) {
					allWindows[allWindows.Count - 1].Add( line );
				} else {
					resultLines.Add( line );
				}
			}
			theseLines.Clear();

			tabCount++;
		}

		if ( element.partColor != currentColor ) {
			currentColor = element.partColor;
			theseLines.Add( "GUI.color = " + ( inCSharp ? "new" : "" ) + " Color( " + element.partColor.r + "f, " + element.partColor.g + "f, " + element.partColor.b + "f, " + element.partColor.a + "f );" );
		}


		switch ( typesOfGUIElement[element.partID] ) {
			case "GUI - Label":
				theseLines.Add( "GUI.Label( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\" );" );
				break;
			case "GUI - Box":
				theseLines.Add( "GUI.Box( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\" );" );
				break;
			case "GUI - Button":
				theseLines.Add( "if ( GUI.Button( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\" ) ) {" );
				theseLines.Add( "\t//DoStuff();" );
				theseLines.Add( "}" );
				break;
			case "GUI - Repeat Button":
				theseLines.Add( "if ( GUI.RepeatButton( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\" ) ) {" );
				theseLines.Add( "\t//DoStuff();" );
				theseLines.Add( "}" );
				break;
			case "GUI - Selection Grid":
				string extraVariable = "someStringArray" + userVariableCounts[27]; //27 is selectiongrid specific
				string someSrings = inCSharp ? "{ " : "[ ";
				for ( int i = 0; i < element.elementSpecificInt2; i++ ) {
					someSrings += "\"x\", ";
				}
				someSrings = someSrings.Remove( someSrings.Length - 2, 2 );
				someSrings += inCSharp ? " }" : " ]";
				userVariableDeclarations.Add( "//This array is the labels for each button in a selection grid, replace the x's with your own stuff, or replace the variable with another collection of strings\n" );
				if ( inCSharp ) {
					userVariableDeclarations.Add( GUIElementsVariableTypeCased[27] + " " + extraVariable + " = new string[" + element.elementSpecificInt2 + "] " + someSrings + ";\n" );
				} else {
					userVariableDeclarations.Add( "var " + extraVariable + " : String[] = " + someSrings + ";\n" );
				}
				userVariableCounts[27]++;
				if ( userVariables.Contains( extraVariable ) ) {
					warnings.Add( "There is more than 1 variable called " + extraVariable + "\n" );
				}
				userVariables.Add( extraVariable );

				theseLines.Add( myVariableName + " = GUI.SelectionGrid( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", " + extraVariable + ", " + element.elementSpecificInt2 + " );" );
				break;
			case "GUI - Toggle":
				theseLines.Add( myVariableName + " = GUI.Toggle( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", \"" + element.label + "\" );" );
				if ( element.groupedElements.Count != 0 ) {
					theseLines.Add( "if( " + myVariableName + ") {" );
				}
				break;
			case "GUI - DrawTexture":
				theseLines.Add( "GUI.DrawTexture( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + " );" );
				break;
			case "GUI - Text Field":
				theseLines.Add( myVariableName + " = GUI.TextField( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + " );" );
				break;
			case "GUI - Text Area":
				theseLines.Add( myVariableName + " = GUI.TextArea( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + " );" );
				break;
			case "GUI - Slider - Horizontal":
				theseLines.Add( myVariableName + " = GUI.HorizontalSlider( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", " + element.elementSpecificFloat1 + "f, " + element.elementSpecificFloat2 + "f);" );
				break;
			case "GUI - Slider - Vertical":
				theseLines.Add( myVariableName + " = GUI.VerticalSlider( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", " + element.elementSpecificFloat1 + "f, " + element.elementSpecificFloat2 + "f);" );
				break;
			case "GUI - Scrollbar - Horizontal":
				theseLines.Add( "//Don't forget to change the 1st float value to the desired thumb size" );
				theseLines.Add( myVariableName + " = GUI.HorizontalScrollbar( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", " + ( Mathf.Abs( element.elementSpecificFloat2 ) + Mathf.Abs( element.elementSpecificFloat1 ) ) / 8f + "f, " + element.elementSpecificFloat1 + "f, " + element.elementSpecificFloat2 + "f);" );
				break;
			case "GUI - Scrollbar - Vertical":
				theseLines.Add( "//Don't forget to change the 1st float value to the desired thumb size" );
				theseLines.Add( myVariableName + " = GUI.VerticalScrollbar( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", " + ( Mathf.Abs( element.elementSpecificFloat2 ) + Mathf.Abs( element.elementSpecificFloat1 ) ) / 8f + "f, " + element.elementSpecificFloat1 + "f, " + element.elementSpecificFloat2 + "f);" );
				break;
			case "GUI - Group":
				theseLines.Add( "GUI.BeginGroup( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + ") );" );
				if ( element.elementSpecificBool ) {
					theseLines.Add( "GUI.Box( " + ( inCSharp ? "new" : "" ) + " Rect( 0, 0, " + element.relativeRect.width + ( element.relativeRect.width % 1 == 0 ? "" : "f" ) + ", " + element.relativeRect.height + ( element.relativeRect.height % 1 == 0 ? "" : "f" ) + "), \"\" );" );
				}
				break;
			case "GUI - Scrollview":
				theseLines.Add( myVariableName + " = GUI.BeginScrollView( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", new Rect(0, 0, " + element.scrollOffset.x + ( element.scrollOffset.x % 1 == 0 ? "" : "f" ) + ", " + element.scrollOffset.y + ( element.scrollOffset.y % 1 == 0 ? "" : "f" ) + ") );" );
				if ( element.elementSpecificBool ) {
					theseLines.Add( "GUI.Box( " + ( inCSharp ? "new" : "" ) + " Rect( 0, 0, " + element.scrollOffset.x + ( element.scrollOffset.x % 1 == 0 ? "" : "" ) + ", " + element.scrollOffset.y + ( element.scrollOffset.y % 1 == 0 ? "" : "f" ) + "), \"\" );" );
				}
				break;
			case "EditorGUI - Shadow Label":
				theseLines.Add( "EditorGUI.DropShadowLabel( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\" );" );
				break;
			case "EditorGUI - Int Field":
				sizeOfThisElement = (int) ( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
				if ( currentEditorLabelSize != sizeOfThisElement ) {
#if UNITY_4_3
					theseLines.Add( "EditorGUIUtility.labelWidth = " + sizeOfThisElement + ";" );
#else
					theseLines.Add( "EditorGUIUtility.LookLikeControls( " + sizeOfThisElement + " );" );
#endif
					currentEditorLabelSize = sizeOfThisElement;
				}
				theseLines.Add( myVariableName + " = EditorGUI.IntField( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				break;
			case "EditorGUI - Float Field":
				sizeOfThisElement = (int) ( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
				if ( currentEditorLabelSize != sizeOfThisElement ) {
#if UNITY_4_3
					theseLines.Add( "EditorGUIUtility.labelWidth = " + sizeOfThisElement + ";" );
#else
					theseLines.Add( "EditorGUIUtility.LookLikeControls( " + sizeOfThisElement + " );" );
#endif
					currentEditorLabelSize = sizeOfThisElement;
				}
				theseLines.Add( myVariableName + " = EditorGUI.FloatField( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				break;
			case "EditorGUI - Object Field":
				sizeOfThisElement = (int) ( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
				if ( currentEditorLabelSize != sizeOfThisElement ) {
#if UNITY_4_3
					theseLines.Add( "EditorGUIUtility.labelWidth = " + sizeOfThisElement + ";" );
#else
					theseLines.Add( "EditorGUIUtility.LookLikeControls( " + sizeOfThisElement + " );" );
#endif
					currentEditorLabelSize = sizeOfThisElement;
				}
				theseLines.Add( "//Change 'Object' to the type of object you need, eg. GameObject, Material. Change false to true to allow scene objects" );
				theseLines.Add( myVariableName + " = EditorGUI.ObjectField( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + ", typeof(Object), false );" );
				break;
			case "EditorGUI - Color Field":
				sizeOfThisElement = (int) ( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
				if ( currentEditorLabelSize != sizeOfThisElement ) {
#if UNITY_4_3
					theseLines.Add( "EditorGUIUtility.labelWidth = " + sizeOfThisElement + ";" );
#else
					theseLines.Add( "EditorGUIUtility.LookLikeControls( " + sizeOfThisElement + " );" );
#endif
					currentEditorLabelSize = sizeOfThisElement;
				}
				theseLines.Add( myVariableName + " = EditorGUI.ColorField( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				break;
			case "EditorGUI - Vector Field":
				theseLines.Add( myVariableName + " = EditorGUI.Vector" + element.elementSpecificInt1 + "Field( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				break;
			case "EditorGUI - Rect Field":
				theseLines.Add( myVariableName + " = EditorGUI.RectField( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				break;
			case "EditorGUI - Enum Popup":
				sizeOfThisElement = (int) ( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
				if ( currentEditorLabelSize != sizeOfThisElement ) {
#if UNITY_4_3
					theseLines.Add( "EditorGUIUtility.labelWidth = " + sizeOfThisElement + ";" );
#else
					theseLines.Add( "EditorGUIUtility.LookLikeControls( " + sizeOfThisElement + " );" );
#endif
					currentEditorLabelSize = sizeOfThisElement;
				}
				if ( inCSharp ) {
					theseLines.Add( myVariableName + " = (" + myVariableName + "Choices) EditorGUI.EnumPopup( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				} else {
					theseLines.Add( myVariableName + " =  EditorGUI.EnumPopup( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), \"" + element.label + "\", " + myVariableName + " );" );
				}
				break;
			case "EditorGUI - Foldout":
				theseLines.Add( myVariableName + " = EditorGUI.Foldout( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + "), " + myVariableName + ", \"" + element.label + "\" );" );
				if ( element.groupedElements.Count != 0 ) {
					theseLines.Add( "if( " + myVariableName + ") {" );
				}
				break;
			case "GUI - Window":
				if ( writingToWindow ) {
					warnings.Add( "//You tried to create a window inside another window, this does not work!" );
					theseLines.Add( "//Note: You tried to start a window inside another window here, this is impossible, instead, you get a group with a skin" );
					theseLines.Add( "GUI.BeginGroup( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + ") );" );
					theseLines.Add( "\tGUI.Box( " + ( inCSharp ? "new" : "" ) + " Rect( 0, 0, " + element.relativeRect.width + ( element.relativeRect.width % 1 == 0 ? "" : "f" ) + ", " + element.relativeRect.height + ( element.relativeRect.height % 1 == 0 ? "" : "f" ) + "), \"" + element.label + "\", \"Window\" );" );
				} else {
					writtenWindow = element;
					writingToWindow = true;
					List<string> newWindow = new List<string>();
					allWindows.Add( newWindow );
				}
				break;
			case "EditorGUI - Window":
				if ( writingToWindow ) {
					warnings.Add( "//You tried to create a window inside another window, this does not work!" );
					theseLines.Add( "//Note: You tried to start a window inside another window here, this is impossible, instead, you get a group with a skin" );
					theseLines.Add( "GUI.BeginGroup( " + ( inCSharp ? "new" : "" ) + " Rect(" + StringifyRect( element ) + ") );" );
					theseLines.Add( "\tGUI.Box( " + ( inCSharp ? "new" : "" ) + " Rect( 0, 0, " + element.relativeRect.width + ( element.relativeRect.width % 1 == 0 ? "" : "f" ) + ", " + element.relativeRect.height + ( element.relativeRect.height % 1 == 0 ? "" : "f" ) + "), \"" + element.label + "\", \"Window\" );" );
				} else {
					writtenWindow = element;
					writingToWindow = true;
					List<string> newWindow = new List<string>();
					allWindows.Add( newWindow );
				}
				break;

			default:
				//Should only be one thing that gets here - the Screen (background) so nothing happens
				break;
		}

		for ( int j = 0; j < theseLines.Count; j++ ) {
			string line = theseLines[j];
			line += "\n";
			for ( int i = 0; i < tabCount; i++ ) {
				line = "\t" + line;
			}

			if ( writingToWindow ) {
				allWindows[allWindows.Count - 1].Add( line );
			} else {
				resultLines.Add( line );
			}
		}
		tabCount++;
		for ( int i = 0; i < element.groupedElements.Count; i++ ) {
			RecursiveComplete( element.groupedElements[i] );
		}
		tabCount--;

		theseLines.Clear();
		switch ( typesOfGUIElement[element.partID] ) {
			case "GUI - Toggle":
				if ( element.groupedElements.Count != 0 ) {
					theseLines.Add( "}" );
				}
				break;
			case "GUI - Group":
				theseLines.Add( "GUI.EndGroup();" );
				break;
			case "GUI - Scrollview":
				theseLines.Add( "GUI.EndScrollView();" );
				break;
			case "EditorGUI - Foldout":
				if ( element.groupedElements.Count != 0 ) {
					theseLines.Add( "}" );
				}
				break;
			case "GUI - Window":
				if ( element == writtenWindow ) {
					windowDeclarations.Add( myVariableName + " = GUI.Window( " + ( allWindows.Count - 1 ) + ", " + myVariableName + ", WindowFunction" + ( allWindows.Count - 1 ) + ", \"" + element.label + "\" );" );
					writingToWindow = false;
				} else {
					theseLines.Add( "GUI.EndGroup();" );
				}
				break;
			case "EditorGUI - Window":
				if ( element == writtenWindow ) {
					windowDeclarations.Add( myVariableName + " = GUI.Window( " + ( allWindows.Count - 1 ) + ", " + myVariableName + ", WindowFunction" + ( allWindows.Count - 1 ) + ", \"" + element.label + "\" );" );
					writingToWindow = false;
				} else {
					theseLines.Add( "GUI.EndGroup();" );
				}
				break;
		}

		for ( int j = 0; j < theseLines.Count; j++ ) {
			string line = theseLines[j];
			line += "\n";
			for ( int i = 0; i < tabCount; i++ ) {
				line = "\t" + line;
			}

			if ( writingToWindow ) {
				allWindows[allWindows.Count - 1].Add( line );
			} else {
				resultLines.Add( line );
			}
		}

		theseLines.Clear();

		if ( element.iAmList ) {
			tabCount--;
			theseLines.Add( "}" );
			for ( int j = 0; j < theseLines.Count; j++ ) {
				string line = theseLines[j];
				line += "\n";
				for ( int i = 0; i < tabCount; i++ ) {
					line = "\t" + line;
				}

				if ( writingToWindow ) {
					allWindows[allWindows.Count - 1].Add( line );
				} else {
					resultLines.Add( line );
				}
			}
		}

		if ( writingToWindow ) {
			allWindows[allWindows.Count - 1].Add( "\n" );
		} else {
			resultLines.Add( "\n" );
		}

	}

	#endregion

	#region Recursive Draw

	void DrawGUIElement ( GUIElement element ) {
		//debugString += "drawing " + element.myName + "\n";
		if ( currentSelectedElements.Contains( element ) && disablePreviews ) { //selection highlight
			GUI.color = Color.red;
			GUI.skin = editorSkin;
			GUI.Box( new Rect( element.relativeRect.x, element.relativeRect.y, element.relativeRect.width, element.relativeRect.height ), "", "PlainBox" );
		}


		GUI.enabled = !disablePreviews;
		GUI.color = element.partColor;
		for ( int listIndex = 0; listIndex < ( element.iAmList ? 4 : 1 ); listIndex++ ) {
			GUI.color = new Color( element.partColor.r, element.partColor.g, element.partColor.b, ( ( 4 - listIndex ) / 4f ) * element.partColor.a );
			GUI.skin = previewSkin ? previewSkin : ( element.partID > 16 ? null : editorSkin );
			element.myGUI( element, element.listOffset * listIndex );


			if ( element.partID == TypesOfGUIElement.GUIGroup ||
				element.partID == TypesOfGUIElement.GUIWindow ||
				element.partID == TypesOfGUIElement.EditorGUIWindow ||
				element.partID == TypesOfGUIElement.SelectMany ) {
				if ( element.groupedElements.Count > 0 ) {
					GUI.BeginGroup( OffsetARect( element.relativeRect, element.listOffset * listIndex ) );
					for ( int i = 0; i < element.groupedElements.Count; i++ ) {
						DrawGUIElement( element.groupedElements[i] );
					}
					GUI.EndGroup();
				}
			} else if ( element.partID == TypesOfGUIElement.GUIToggle || element.partID == TypesOfGUIElement.EditorGUIFoldout ) {
				if ( element.elementSpecificBool ) {
					for ( int i = 0; i < element.groupedElements.Count; i++ ) {
						DrawGUIElement( element.groupedElements[i] );
					}
				}
			} else if ( element.partID == TypesOfGUIElement.GUIScrollview ) {
				GUI.BeginScrollView( OffsetARect( element.relativeRect, element.listOffset * listIndex ), someScrollView, new Rect( 0, 0, element.scrollOffset.x, element.scrollOffset.y ) );
				for ( int i = 0; i < element.groupedElements.Count; i++ ) {
					DrawGUIElement( element.groupedElements[i] );
				}
				GUI.EndScrollView();
			}

			if ( element.iAmList && chosenGUIPart != TypesOfGUIElement.SelectMany && currentSelectedElements[0] == element ) { //add the list handle
				GUI.enabled = true;
				Rect myScreenspaceRect = element.relativeRect;
				GUI.color = Color.white;
				GUI.skin = editorSkin;
				GUI.Box( new Rect( myScreenspaceRect.x + element.listOffset.x - 6, myScreenspaceRect.y + element.listOffset.y - 6, 12, 12 ), "", "Handle" );
				GUI.enabled = !disablePreviews;
			}
		}

	}

	#endregion

	#region GUIFunctions

	void GNothing ( GUIElement element, Vector2 offset ) {
		//This is the window, nothing happens
	}

	void GLabel ( GUIElement element, Vector2 offset ) {
		GUI.skin = editorSkin;
		if ( disablePreviews ) GUI.Box( OffsetARect( element.relativeRect, offset ), "", "PlainBoxEmpty" );
		GUI.skin = previewSkin ? previewSkin : editorSkin;
		GUI.Label( OffsetARect( element.relativeRect, offset ), element.label );
	}

	void GBox ( GUIElement element, Vector2 offset ) {
		GUI.Box( OffsetARect( element.relativeRect, offset ), element.label );
	}

	void GButton ( GUIElement element, Vector2 offset ) {
		GUI.Button( OffsetARect( element.relativeRect, offset ), element.label );
	}

	void GRepeatButton ( GUIElement element, Vector2 offset ) {
		GUI.RepeatButton( OffsetARect( element.relativeRect, offset ), element.label );
	}

	void GSelectionGrid ( GUIElement element, Vector2 offset ) {
		GUI.SelectionGrid( OffsetARect( element.relativeRect, offset ), someInt, MakeStrings( element.elementSpecificInt1 ), element.elementSpecificInt2 );
	}

	void GToggle ( GUIElement element, Vector2 offset ) {
		GUI.skin = editorSkin;
		if ( disablePreviews ) GUI.Box( OffsetARect( element.relativeRect, offset ), "", "PlainBoxEmpty" );
		GUI.skin = previewSkin ? previewSkin : editorSkin;
		element.elementSpecificBool = GUI.Toggle( OffsetARect( element.relativeRect, offset ), element.elementSpecificBool, element.label );
	}

	void GDrawTexture ( GUIElement element, Vector2 offset ) {
		GUI.skin = editorSkin;
		GUI.Box( OffsetARect( element.relativeRect, offset ), "", "Textured" );
		GUI.skin = previewSkin ? previewSkin : editorSkin;
	}

	void GTextField ( GUIElement element, Vector2 offset ) {
		GUI.TextField( OffsetARect( element.relativeRect, offset ), "Text Field (single-line)" );
	}

	void GTextArea ( GUIElement element, Vector2 offset ) {
		GUI.TextArea( OffsetARect( element.relativeRect, offset ), "Text Area (multi-line)" );
	}

	void GSliderHorizontal ( GUIElement element, Vector2 offset ) {
		GUI.HorizontalSlider( OffsetARect( element.relativeRect, offset ), ( element.elementSpecificFloat1 + element.elementSpecificFloat2 ) / 2f, element.elementSpecificFloat1, element.elementSpecificFloat2 );
	}

	void GSliderVertical ( GUIElement element, Vector2 offset ) {
		GUI.VerticalSlider( OffsetARect( element.relativeRect, offset ), ( element.elementSpecificFloat1 + element.elementSpecificFloat2 ) / 2f, element.elementSpecificFloat1, element.elementSpecificFloat2 );
	}

	void GScrollbarHorizontal ( GUIElement element, Vector2 offset ) {
		GUI.HorizontalScrollbar( OffsetARect( element.relativeRect, offset ), ( element.elementSpecificFloat1 + element.elementSpecificFloat2 ) / 2f, ( Mathf.Abs( element.elementSpecificFloat2 ) + Mathf.Abs( element.elementSpecificFloat1 ) ) / 8f, element.elementSpecificFloat1, element.elementSpecificFloat2 );
	}

	void GScrollbarVertical ( GUIElement element, Vector2 offset ) {
		GUI.VerticalScrollbar( OffsetARect( element.relativeRect, offset ), ( element.elementSpecificFloat1 + element.elementSpecificFloat2 ) / 2f, ( Mathf.Abs( element.elementSpecificFloat2 ) + Mathf.Abs( element.elementSpecificFloat1 ) ) / 8f, element.elementSpecificFloat1, element.elementSpecificFloat2 );
	}

	void GGroup ( GUIElement element, Vector2 offset ) {
		if ( element.elementSpecificBool ) GUI.Box( OffsetARect( element.relativeRect, offset ), "" );
		GUI.skin = editorSkin;
		if ( disablePreviews ) GUI.Box( OffsetARect( element.relativeRect, offset ), "", "PlainBoxEmpty" );
		GUI.skin = previewSkin ? previewSkin : editorSkin;
	}

	void GScrollview ( GUIElement element, Vector2 offset ) {
		if ( element.elementSpecificBool ) GUI.Box( OffsetARect( element.relativeRect, offset ), "" );

		GUI.skin = editorSkin;
		if ( disablePreviews ) GUI.Box( OffsetARect( element.relativeRect, offset ), "", "PlainBoxEmpty" );
		GUI.BeginGroup( OffsetARect( element.relativeRect, offset ) );
		if ( element == currentSelectedElements[0] && currentSelectedElements.Count == 1 ) {
			GUI.color = Color.green;
			if ( disablePreviews ) GUI.Box( new Rect( 0, 0, element.scrollOffset.x, element.scrollOffset.y ), "", "PlainBoxTint" );
			GUI.enabled = true;
			GUI.color = Color.white;
			GUI.Box( new Rect( element.scrollOffset.x - 6, element.scrollOffset.y - 6, 12, 12 ), "", "Handle" );
			GUI.enabled = !disablePreviews;
		}
		GUI.skin = previewSkin ? previewSkin : editorSkin;

		GUI.EndGroup();
	}

	void GWindow ( GUIElement element, Vector2 offset ) {
		debugString += "Window at " + ( OffsetARect( element.relativeRect, offset ) ) + "\n";
		GUI.Box( OffsetARect( element.relativeRect, offset ), element.label, "Window" );
	}

	void GShadowLabel ( GUIElement element, Vector2 offset ) {
		GUI.skin = null;
		EditorGUI.DropShadowLabel( OffsetARect( element.relativeRect, offset ), element.label );
	}

	void GIntField ( GUIElement element, Vector2 offset ) {
#if UNITY_4_3
		EditorGUIUtility.labelWidth = element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2;
#else
		EditorGUIUtility.LookLikeControls( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
#endif
		EditorGUI.IntField( OffsetARect( element.relativeRect, offset ), element.label, someInt );
	}

	void GFloatField ( GUIElement element, Vector2 offset ) {
#if UNITY_4_3
		EditorGUIUtility.labelWidth = element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2;
#else
		EditorGUIUtility.LookLikeControls( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
#endif
		EditorGUI.FloatField( OffsetARect( element.relativeRect, offset ), element.label, someFloat );
	}

	void GObjectField ( GUIElement element, Vector2 offset ) {
#if UNITY_4_3
		EditorGUIUtility.labelWidth = element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2;
#else
		EditorGUIUtility.LookLikeControls( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
#endif
		EditorGUI.ObjectField( OffsetARect( element.relativeRect, offset ), element.label, someObject, typeof( Object ), false );
	}

	void GColorField ( GUIElement element, Vector2 offset ) {
#if UNITY_4_3
		EditorGUIUtility.labelWidth = element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2;
#else
		EditorGUIUtility.LookLikeControls( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
#endif
		EditorGUI.ColorField( OffsetARect( element.relativeRect, offset ), element.label, someColor );
	}

	void GVectorField ( GUIElement element, Vector2 offset ) {
		switch ( element.elementSpecificInt1 ) {
			case 2:
				someVector = EditorGUI.Vector2Field( OffsetARect( element.relativeRect, offset ), element.label, someVector );
				break;
			case 3:
				someVector = EditorGUI.Vector3Field( OffsetARect( element.relativeRect, offset ), element.label, someVector );
				break;
			case 4:
				someVector = EditorGUI.Vector4Field( OffsetARect( element.relativeRect, offset ), element.label, someVector );
				break;
		}
	}

	void GRectField ( GUIElement element, Vector2 offset ) {
		someRect = EditorGUI.RectField( OffsetARect( element.relativeRect, offset ), element.label, someRect );
	}

	void GEnumPopup ( GUIElement element, Vector2 offset ) {
#if UNITY_4_3
		EditorGUIUtility.labelWidth = element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2;
#else
		EditorGUIUtility.LookLikeControls( element.elementSpecificBool ? element.elementSpecificInt1 : element.relativeRect.width / 2 );
#endif
		EditorGUI.EnumPopup( OffsetARect( element.relativeRect, offset ), element.label, someEnum.Choices );
	}

	void GFoldout ( GUIElement element, Vector2 offset ) {
		GUI.skin = editorSkin;
		if ( disablePreviews ) GUI.Box( OffsetARect( element.relativeRect, offset ), "", "PlainBoxEmpty" );
		GUI.skin = null;
		element.elementSpecificBool = GUI.Toggle( OffsetARect( element.relativeRect, offset ), element.elementSpecificBool, element.label, "Foldout" );
	}

	void GEditorWindow ( GUIElement element, Vector2 offset ) {
		GUI.Box( OffsetARect( element.relativeRect, offset ), element.label, "Window" );
	}

	#endregion

	#region Helper Functions

	void RecursiveReasses ( GUIElement element ) {
		allElements.Add( element );
		for ( int i = 0; i < element.groupedElements.Count; i++ ) {
			RecursiveReasses( element.groupedElements[i] );
		}
	}

	void ReassesAllElements () {
		allElements = new List<GUIElement>();
		RecursiveReasses( screenElement );
	}

	bool ContainsAnyParent ( GUIElement element ) {
		if ( element == screenElement ) return false;
		if ( currentSelectedElements.Contains( element ) ) {
			return true;
		} else {
			return ContainsAnyParent( element.containingElement );
		}
	}

	GUIElement RecursiveCopy ( GUIElement original, GUIElement parent, GUIElement caller ) {
		GUIElement newElement = new GUIElement( original );
		newElement.containingElement = parent;
		if ( haveASelectionGroup ) {
			if ( !ContainsAnyParent( newElement ) ) currentSelectedElements.Add( newElement );
		}
		newElement.groupedElements = new List<GUIElement>();
		if ( original == caller ) {
			newElement.containingElement.groupedElements.Insert( newElement.containingElement.groupedElements.IndexOf( caller ) + 1, newElement );
		} else {
			newElement.containingElement.groupedElements.Add( newElement );
		}

		for ( int i = 0; i < original.groupedElements.Count; i++ ) {
			if ( safety > 512 ) {
				Debug.Log( "Managed to loop a lot" );
				break;
			}
			safety++;
			RecursiveCopy( original.groupedElements[i], newElement, caller );
		}

		return newElement;
	}

	void CopyElements ( bool viaButton ) {
		safety = 0;
		if ( haveASelectionGroup ) {
			List<GUIElement> temp = new List<GUIElement>( currentSelectedElements );
			currentSelectedElements.Clear();
			for ( int i = temp.Count - 1; i > -1; i-- ) {
				if ( temp.Contains( temp[i].containingElement ) ) {
					temp.RemoveAt( i );
				}
			}
			for ( int i = 0; i < temp.Count; i++ ) {
				RecursiveCopy( temp[i], temp[i].containingElement, temp[i] );
			}
			foreach ( GUIElement element in currentSelectedElements ) {
				if ( currentSelectedElements.Contains( element.containingElement ) && ( element.containingElement.partID != TypesOfGUIElement.GUIToggle && element.containingElement.partID != TypesOfGUIElement.EditorGUIFoldout ) ) continue;
				if ( viaButton ) element.relativeRect = OffsetARect( element.relativeRect, new Vector2( 15, 0 ) );
			}
			completeGroupRect.x += 15;
		} else {
			currentSelectedElements[0] = RecursiveCopy( currentSelectedElements[0], currentSelectedElements[0].containingElement, currentSelectedElements[0] );
			if ( viaButton && !haveASelectionGroup ) currentSelectedElements[0].relativeRect.x += 15;
		}
		ReassesAllElements();

		HideContext();
		return;
	}

	void CollectDeleted ( GUIElement element ) {
		elementsToDelete.Add( element );
		foreach ( GUIElement child in element.groupedElements ) {
			CollectDeleted( child );
		}
	}

	void DeleteElements () {

		elementsToDelete = new List<GUIElement>();
		for ( int i = 0; i < currentSelectedElements.Count; i++ ) {
			CollectDeleted( currentSelectedElements[i] );
		}
		foreach ( GUIElement del in elementsToDelete ) {
			del.containingElement.groupedElements.Remove( del );
		}
		ReassesAllElements();

		ClearSelection();
		haveASelectionGroup = false;
		completeGroupRect = new Rect( 0, 0, 0, 0 );
		HideContext();
		pointingAtElement = false;
	}

	void ClearSelection () {
		currentSelectedElements.Clear();
		currentSelectedElements.Add( null );
		selectedElementIndex = -1;
	}

	string[] MakeStrings ( int length ) {
		string[] strings = new string[length];
		for ( int i = 0; i < length; i++ ) {
			strings[i] = i % 2 == 0 ? "Selection" : "Grid";
		}
		return strings;
	}

	void ReFocusGUI ( string nameOfControl ) {
		waitingToFocus = true;
		focusThisControl = nameOfControl;
	}

	Rect ReverseOffsetARect ( Rect r1, Rect r2 ) {
		return new Rect( r1.x - r2.x, r1.y - r2.y, r1.width, r1.height );
	}
	Rect OffsetARect ( Rect r1, Rect r2 ) {
		return new Rect( r1.x + r2.x, r1.y + r2.y, r1.width, r1.height );
	}
	Rect OffsetARect ( Rect r1, Vector2 r2 ) {
		return new Rect( r1.x + r2.x, r1.y + r2.y, r1.width, r1.height );
	}
	Rect OffsetARect ( Rect r1, Rect r2, Rect r3 ) {
		return new Rect( r1.x + r2.x - r3.x, r1.y + r2.y - r3.y, r1.width, r1.height );
	}

	Rect RelativeRectFromScreenRect ( GUIElement element, Rect inScreen ) {
		if ( element.containingElement == screenElement ) {
			return inScreen;
		} else {
			if ( element.containingElement.partID == TypesOfGUIElement.GUIToggle || element.containingElement.partID == TypesOfGUIElement.EditorGUIFoldout ) {
				return RelativeRectFromScreenRect( element.containingElement, inScreen );
			} else {
				return ReverseOffsetARect( inScreen, ScreenspaceRect( element.containingElement ) );
			}
		}
	}

	GUIElement FirstNonToggle ( GUIElement element ) {
		if ( element.partID == TypesOfGUIElement.GUIToggle || element.partID == TypesOfGUIElement.EditorGUIFoldout ) {
			return ( FirstNonToggle( element.containingElement ) );
		} else {
			return element;
		}
	}

	Rect ScreenspaceRect ( GUIElement element ) {
		if ( element.containingElement != null && element.containingElement != screenElement ) {
			if ( element.containingElement.partID == TypesOfGUIElement.GUIToggle || element.containingElement.partID == TypesOfGUIElement.EditorGUIFoldout ) {
				return OffsetARect( element.relativeRect, ScreenspaceRect( FirstNonToggle( element.containingElement ) ) );
			} else {
				return OffsetARect( element.relativeRect, ScreenspaceRect( element.containingElement ) );
			}
		}
		return element.relativeRect;
	}

	Rect AbsoluteRect ( GUIElement element ) {
		Rect inScreen = ScreenspaceRect( element );
		return OffsetARect( inScreen, screenRect );
	}

	Vector2 SnapGroupPos ( Vector2 v ) {
		if ( !snapDisplay && !snapKB ) return v;
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( currentSelectedElements.Contains( allElements[j] ) ) continue;
			Vector2[] RCorner = new Vector2[] { new Vector2( allElements[j].relativeRect.x, allElements[j].relativeRect.y ), new Vector2( allElements[j].relativeRect.xMax, allElements[j].relativeRect.y ), new Vector2( allElements[j].relativeRect.x, allElements[j].relativeRect.yMax ), new Vector2( allElements[j].relativeRect.xMax, allElements[j].relativeRect.yMax ) };
			foreach ( Vector2 C in RCorner ) {
				if ( Vector2.Distance( v, C ) < 10 ) {
					return C;
				}
			}
		}
		return SnapGroupPosSides( v );
	}

	Vector2 SnapGroupPosSides ( Vector2 v ) {
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( currentSelectedElements.Contains( allElements[j] ) ) continue;
			float[] testSides = new float[] { allElements[j].relativeRect.x, allElements[j].relativeRect.y, allElements[j].relativeRect.xMax, allElements[j].relativeRect.yMax };
			for ( int testSide = 0; testSide < 4; testSide++ ) {
				if ( Mathf.Abs( testSides[testSide] - v[testSide % 2] ) < 6 ) {
					switch ( testSide ) {
						case 0://left side of testRect
							if ( ( v[1] > testSides[3] && v[1] > testSides[3] ) || ( v[1] < testSides[1] && v[1] < testSides[1] ) ) continue;
							v.x = testSides[testSide];
							break;
						case 1://top side of testRect
							if ( ( v[0] > testSides[2] && v[0] > testSides[2] ) || ( v[0] < testSides[0] && v[0] < testSides[0] ) ) continue;
							v.y = testSides[testSide];
							break;
						case 2://right side of testRect
							if ( ( v[1] > testSides[3] && v[1] > testSides[3] ) || ( v[1] < testSides[1] && v[1] < testSides[1] ) ) continue;
							v.x = testSides[testSide];
							break;
						case 3://bottom side of testRect
							if ( ( v[0] > testSides[2] && v[0] > testSides[2] ) || ( v[0] < testSides[0] && v[0] < testSides[0] ) ) continue;
							v.y = testSides[testSide];
							break;

					}
					return v;
				}
			}
		}
		return v;
	}

	Vector2 SnapPos ( Vector2 v, int ind ) {
		if ( !snapDisplay && !snapKB ) return v;
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( j == ind ) continue;
			Vector2[] RCorner = new Vector2[] { new Vector2( allElements[j].relativeRect.x, allElements[j].relativeRect.y ), new Vector2( allElements[j].relativeRect.xMax, allElements[j].relativeRect.y ), new Vector2( allElements[j].relativeRect.x, allElements[j].relativeRect.yMax ), new Vector2( allElements[j].relativeRect.xMax, allElements[j].relativeRect.yMax ) };
			foreach ( Vector2 C in RCorner ) {
				if ( Vector2.Distance( v, C ) < 10 ) {
					return C;
				}
			}
		}
		return SnapPosSides( v, ind );
	}

	Vector2 SnapPosSides ( Vector2 v, int ind ) {
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( j == ind ) continue;
			float[] testSides = new float[] { allElements[j].relativeRect.x, allElements[j].relativeRect.y, allElements[j].relativeRect.xMax, allElements[j].relativeRect.yMax };
			for ( int testSide = 0; testSide < 4; testSide++ ) {
				if ( Mathf.Abs( testSides[testSide] - v[testSide % 2] ) < 6 ) {
					switch ( testSide ) {
						case 0://left side of testRect
							if ( ( v[1] > testSides[3] && v[1] > testSides[3] ) || ( v[1] < testSides[1] && v[1] < testSides[1] ) ) continue;
							v.x = testSides[testSide];
							break;
						case 1://top side of testRect
							if ( ( v[0] > testSides[2] && v[0] > testSides[2] ) || ( v[0] < testSides[0] && v[0] < testSides[0] ) ) continue;
							v.y = testSides[testSide];
							break;
						case 2://right side of testRect
							if ( ( v[1] > testSides[3] && v[1] > testSides[3] ) || ( v[1] < testSides[1] && v[1] < testSides[1] ) ) continue;
							v.x = testSides[testSide];
							break;
						case 3://bottom side of testRect
							if ( ( v[0] > testSides[2] && v[0] > testSides[2] ) || ( v[0] < testSides[0] && v[0] < testSides[0] ) ) continue;
							v.y = testSides[testSide];
							break;

					}
					return v;
				}
			}
		}
		return v;
	}

	Rect SnapGroup () {
		Vector2[] rCorner = new Vector2[] { new Vector2( completeGroupRect.x, completeGroupRect.y ), new Vector2( completeGroupRect.xMax, completeGroupRect.y ), new Vector2( completeGroupRect.x, completeGroupRect.yMax ), new Vector2( completeGroupRect.xMax, completeGroupRect.yMax ) };
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( currentSelectedElements.Contains( allElements[j] ) ) continue;
			Rect matchRect = ScreenspaceRect( allElements[j] );
			Vector2[] RCorner = new Vector2[] { new Vector2( matchRect.x, matchRect.y ), new Vector2( matchRect.xMax, matchRect.y ), new Vector2( matchRect.x, matchRect.yMax ), new Vector2( matchRect.xMax, matchRect.yMax ) };
			foreach ( Vector2 C in RCorner ) {
				for ( int i = 0; i < 4; i++ ) {
					if ( Vector2.Distance( rCorner[i], C ) < 10 ) {
						Rect resultRect = new Rect();
						switch ( i ) {
							case 0:
								resultRect = new Rect( C.x, C.y, completeGroupRect.width, completeGroupRect.height );
								break;
							case 1:
								resultRect = new Rect( C.x - completeGroupRect.width, C.y, completeGroupRect.width, completeGroupRect.height );
								break;
							case 2:
								resultRect = new Rect( C.x, C.y - completeGroupRect.height, completeGroupRect.width, completeGroupRect.height );
								break;
							case 3:
								resultRect = new Rect( C.x - completeGroupRect.width, C.y - completeGroupRect.height, completeGroupRect.width, completeGroupRect.height );
								break;
						}
						return resultRect;
					}
				}
			}
		}
		return SnapGroupSides();
	}

	Rect SnapGroupSides () {
		float[] snapRectSides = new float[] { completeGroupRect.x, completeGroupRect.y, completeGroupRect.xMax, completeGroupRect.yMax };
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( currentSelectedElements.Contains( allElements[j] ) ) continue;
			Rect matchRect = ScreenspaceRect( allElements[j] );
			float[] testSides = new float[] { matchRect.x, matchRect.y, matchRect.xMax, matchRect.yMax };
			for ( int testSide = 0; testSide < 4; testSide++ ) {
				bool snappedIn = false, snappedOut = false;
				if ( Mathf.Abs( snapRectSides[testSide] - testSides[testSide] ) < 6 ) {
					snappedIn = true;
				}
				if ( Mathf.Abs( snapRectSides[testSide] - testSides[( testSide + 2 ) % 4] ) < 6 ) {
					snappedOut = true;
				}
				if ( snappedIn || snappedOut ) {
					Rect resultRect = new Rect( completeGroupRect );
					switch ( testSide ) {
						case 0://left side of snaprect
							if ( ( snapRectSides[1] > testSides[3] && snapRectSides[3] > testSides[3] ) || ( snapRectSides[1] < testSides[1] && snapRectSides[3] < testSides[1] ) ) continue;
							resultRect.x = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4];
							break;
						case 1://top side of snaprect
							if ( ( snapRectSides[0] > testSides[2] && snapRectSides[2] > testSides[2] ) || ( snapRectSides[0] < testSides[0] && snapRectSides[2] < testSides[0] ) ) continue;
							resultRect.y = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4];
							break;
						case 2://right side of snaprect
							if ( ( snapRectSides[1] > testSides[3] && snapRectSides[3] > testSides[3] ) || ( snapRectSides[1] < testSides[1] && snapRectSides[3] < testSides[1] ) ) continue;
							resultRect.x = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4] - resultRect.width;
							break;
						case 3://bottom side of snaprect
							if ( ( snapRectSides[0] > testSides[2] && snapRectSides[2] > testSides[2] ) || ( snapRectSides[0] < testSides[0] && snapRectSides[2] < testSides[0] ) ) continue;
							resultRect.y = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4] - resultRect.height;
							break;

					}
					return resultRect;
				}
			}
		}
		return completeGroupRect;
	}

	Rect SnapRect ( GUIElement element, int ind ) {
		Rect thisRect = ScreenspaceRect( element );
		Vector2[] rCorner = new Vector2[] { new Vector2( thisRect.x, thisRect.y ), new Vector2( thisRect.xMax, thisRect.y ), new Vector2( thisRect.x, thisRect.yMax ), new Vector2( thisRect.xMax, thisRect.yMax ) };
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( j == ind ) { continue; }
			Rect matchRect = ScreenspaceRect( allElements[j] );
			Vector2[] RCorner = new Vector2[] { new Vector2( matchRect.x, matchRect.y ), new Vector2( matchRect.xMax, matchRect.y ), new Vector2( matchRect.x, matchRect.yMax ), new Vector2( matchRect.xMax, matchRect.yMax ) };
			foreach ( Vector2 C in RCorner ) {
				for ( int i = 0; i < 4; i++ ) {
					if ( Vector2.Distance( rCorner[i], C ) < 10 ) {
						Rect resultRect = new Rect();
						switch ( i ) {
							case 0:
								resultRect = new Rect( C.x, C.y, thisRect.width, thisRect.height );
								break;
							case 1:
								resultRect = new Rect( C.x - thisRect.width, C.y, thisRect.width, thisRect.height );
								break;
							case 2:
								resultRect = new Rect( C.x, C.y - thisRect.height, thisRect.width, thisRect.height );
								break;
							case 3:
								resultRect = new Rect( C.x - thisRect.width, C.y - thisRect.height, thisRect.width, thisRect.height );
								break;
						}
						resultRect = ReverseOffsetARect( resultRect, ScreenspaceRect( element.containingElement ) );
						return resultRect;
					}
				}
			}
		}
		return SnapRectSides( element, ind );
	}

	Rect SnapRectSides ( GUIElement element, int ind ) {
		Rect thisRect = ScreenspaceRect( element );
		float[] snapRectSides = new float[] { thisRect.x, thisRect.y, thisRect.xMax, thisRect.yMax };
		for ( int j = 0; j < allElements.Count; j++ ) {
			if ( j == ind ) continue;
			Rect matchRect = ScreenspaceRect( allElements[j] );
			float[] testSides = new float[] { matchRect.x, matchRect.y, matchRect.xMax, matchRect.yMax };
			for ( int testSide = 0; testSide < 4; testSide++ ) {
				bool snappedIn = false, snappedOut = false;
				if ( Mathf.Abs( snapRectSides[testSide] - testSides[testSide] ) < 6 ) {
					snappedIn = true;
				}
				if ( Mathf.Abs( snapRectSides[testSide] - testSides[( testSide + 2 ) % 4] ) < 6 ) {
					snappedOut = true;
				}
				if ( snappedIn || snappedOut ) {
					Rect resultRect = new Rect( thisRect );
					switch ( testSide ) {
						case 0://left side of snaprect
							if ( ( snapRectSides[1] > testSides[3] && snapRectSides[3] > testSides[3] ) || ( snapRectSides[1] < testSides[1] && snapRectSides[3] < testSides[1] ) ) continue;
							resultRect.x = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4];
							break;
						case 1://top side of snaprect
							if ( ( snapRectSides[0] > testSides[2] && snapRectSides[2] > testSides[2] ) || ( snapRectSides[0] < testSides[0] && snapRectSides[2] < testSides[0] ) ) continue;
							resultRect.y = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4];
							break;
						case 2://right side of snaprect
							if ( ( snapRectSides[1] > testSides[3] && snapRectSides[3] > testSides[3] ) || ( snapRectSides[1] < testSides[1] && snapRectSides[3] < testSides[1] ) ) continue;
							resultRect.x = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4] - resultRect.width;
							break;
						case 3://bottom side of snaprect
							if ( ( snapRectSides[0] > testSides[2] && snapRectSides[2] > testSides[2] ) || ( snapRectSides[0] < testSides[0] && snapRectSides[2] < testSides[0] ) ) continue;
							resultRect.y = testSides[snappedIn ? testSide : ( testSide + 2 ) % 4] - resultRect.height;
							break;

					}
					resultRect = ReverseOffsetARect( resultRect, ScreenspaceRect( element.containingElement ) );
					return resultRect;
				}
			}
		}
		return element.relativeRect;
	}

	void HideContext () {
		showContext = false;
		ReFocusGUI( "Dummy" );
		contextRect.width = contextRect.height = 0;
	}

	string DisplayRect ( GUIElement element ) {
		string s = "x:";
		int dx = (int) element.anchoredTo % 3;
		int dy = (int) element.anchoredTo / 3;
		switch ( dx ) {
			case 0:
				s += element.relativeRect.x;
				break;
			case 1:
				s += "(Screen.width / 2f) - " + ( ( screenSizeX / 2f ) - element.relativeRect.x );
				break;
			case 2:
				s += "Screen.width - " + ( screenSizeX - element.relativeRect.x );
				break;
		}
		s += ", y:";
		switch ( dy ) {
			case 0:
				s += element.relativeRect.y;
				break;
			case 1:
				s += "(Screen.height / 2f) - " + ( ( screenSizeY / 2f ) - element.relativeRect.y );
				break;
			case 2:
				s += "Screen.height - " + ( screenSizeY - element.relativeRect.y );
				break;
		}
		s = s.Replace( "- -", "+ " );
		s += ", width:" + element.relativeRect.width + ", height:" + element.relativeRect.height;
		return s;
	}

	string StringifyRect ( GUIElement element ) {
		string s = "";
		int dx = (int) element.anchoredTo % 3;
		int dy = (int) element.anchoredTo / 3;
		switch ( dx ) {
			case 0:
				s += element.relativeRect.x;
				break;
			case 1:
				s += "(Screen.width / 2f) - " + ( ( screenSizeX / 2f ) - element.relativeRect.x );
				if ( screenSizeX % 2 != 0 ) s += "f";
				break;
			case 2:
				s += "Screen.width - " + ( screenSizeX - element.relativeRect.x );
				break;
		}
		if ( element.relativeRect.x % 1 != 0 ) s += "f";
		if ( element.iAmList && element.listOffset.x != 0 ) {
			s += " + " + element.listOffset.x;
			if ( element.listOffset.x % 1 != 0 ) s += "f";
			s += " * list" + listCount + "Index";
		}
		s += ", ";
		switch ( dy ) {
			case 0:
				s += element.relativeRect.y;
				break;
			case 1:
				s += "(Screen.height / 2f) - " + ( ( screenSizeY / 2f ) - element.relativeRect.y );
				if ( screenSizeY % 2 != 0 ) s += "f";
				break;
			case 2:
				s += "Screen.height - " + ( screenSizeY - element.relativeRect.y );
				break;
		}
		s = s.Replace( "- -", "+ " );
		if ( element.relativeRect.y % 1 != 0 ) s += "f";
		if ( element.iAmList && element.listOffset.y != 0 ) {
			s += " + " + element.listOffset.y;
			if ( element.listOffset.y % 1 != 0 ) s += "f";
			s += " * list" + listCount + "Index";
		}
		s += ", " + element.relativeRect.width;
		if ( element.relativeRect.width % 1 != 0 ) s += "f";
		s += ", " + element.relativeRect.height;
		if ( element.relativeRect.height % 1 != 0 ) s += "f";
		return s;
	}


	void ReGroupElements ( int testingIndex ) {
		insertingToGroup = false;
		if ( !currentSelectedElements.Contains( allElements[testingIndex] ) ) {
			if ( allElements[testingIndex].partID == TypesOfGUIElement.SelectMany ||
				allElements[testingIndex].partID == TypesOfGUIElement.GUIGroup ||
				allElements[testingIndex].partID == TypesOfGUIElement.GUIWindow ||
				allElements[testingIndex].partID == TypesOfGUIElement.GUIToggle ||
				allElements[testingIndex].partID == TypesOfGUIElement.EditorGUIFoldout ||
				allElements[testingIndex].partID == TypesOfGUIElement.EditorGUIWindow ||
				allElements[testingIndex].partID == TypesOfGUIElement.GUIScrollview ) {


				if ( haveASelectionGroup ) {

					List<GUIElement> temp = new List<GUIElement>( currentSelectedElements );
					for ( int i = temp.Count - 1; i > -1; i-- ) {
						if ( temp.Contains( temp[i].containingElement ) ) {
							temp.RemoveAt( i );
						}
					}
					for ( int i = 0; i < temp.Count; i++ ) {
						temp[i].relativeRect = ScreenspaceRect( temp[i] );
						temp[i].containingElement.groupedElements.Remove( temp[i] );
						temp[i].containingElement = allElements[testingIndex];
						temp[i].containingElement.groupedElements.Add( temp[i] );
						temp[i].relativeRect = ReverseOffsetARect( temp[i].relativeRect, ScreenspaceRect( FirstNonToggle( temp[i].containingElement ) ) );
						RecursiveFixRelatives( temp[i], temp[i].containingElement );
					}
				} else {
					currentSelectedElements[0].relativeRect = ScreenspaceRect( currentSelectedElements[0] );
					currentSelectedElements[0].containingElement.groupedElements.Remove( currentSelectedElements[0] );
					currentSelectedElements[0].containingElement = allElements[testingIndex];
					currentSelectedElements[0].containingElement.groupedElements.Add( currentSelectedElements[0] );
					currentSelectedElements[0].relativeRect = ReverseOffsetARect( currentSelectedElements[0].relativeRect, ScreenspaceRect( FirstNonToggle( currentSelectedElements[0].containingElement ) ) );
					RecursiveFixRelatives( currentSelectedElements[0], currentSelectedElements[0].containingElement );
					ReassesAllElements();
				}
			}
		}
	}

	void RecursiveFixRelatives ( GUIElement element, GUIElement newGroup ) {
		if ( element.containingElement.partID == TypesOfGUIElement.GUIToggle || element.containingElement.partID == TypesOfGUIElement.EditorGUIFoldout ) {
			if ( element != newGroup ) {
				element.relativeRect = ReverseOffsetARect( element.relativeRect, ScreenspaceRect( newGroup ) );
			}
		}
		if ( element.partID == TypesOfGUIElement.GUIToggle || element.partID == TypesOfGUIElement.EditorGUIFoldout ) {
			foreach ( GUIElement child in element.groupedElements ) {
				RecursiveFixRelatives( child, newGroup );
			}
		}
	}

	bool ColorsMatch () {
		Color test = currentSelectedElements[0].partColor;
		for ( int i = 1; i < currentSelectedElements.Count; i++ ) {
			if ( test != currentSelectedElements[i].partColor ) return false;
		}
		return true;
	}

	Rect GrowSelection () {
		Rect R = new Rect( ScreenspaceRect( currentSelectedElements[0] ) );
		for ( int i = 1; i < currentSelectedElements.Count; i++ ) {
			Rect r = ScreenspaceRect( currentSelectedElements[i] );
			if ( r.x < R.x ) {
				R.width += R.x - r.x;
				R.x = r.x;
			}
			if ( r.y < R.y ) {
				R.height += R.y - r.y;
				R.y = r.y;
			}
			if ( r.xMax > R.xMax ) R.xMax = r.xMax;
			if ( r.yMax > R.yMax ) R.yMax = r.yMax;
		}
		return R;
	}



	void CollectSelection () {
		if ( !appendingSelection ) currentSelectedElements.Clear();
		for ( int i = 0; i < allElements.Count; i++ ) {
			if ( RectInsideRect( selectingRect, ScreenspaceRect( allElements[i] ) ) ) {
				currentSelectedElements.Add( allElements[i] );
			}
		}
		if ( !appendingSelection && currentSelectedElements.Count == 0 ) {
			currentSelectedElements.Add( null );
		}
	}

	bool RectInsideRect ( Rect selection, Rect test ) {
		if ( selection.width < 0 ) selection.x -= ( selection.width *= -1 );
		if ( selection.height < 0 ) selection.y -= ( selection.height *= -1 );
		return selection.x < test.x && selection.xMax > test.xMax && selection.y < test.y && selection.yMax > test.yMax;

	}

	bool AllOfType ( params int[] types ) {
		foreach ( GUIElement element in currentSelectedElements ) {
			if ( System.Array.IndexOf( types, element.partID ) == -1 ) return false;
		}
		return true;
	}

	void RoundRect ( GUIElement element ) {
		if ( !IntRounding ) return;
		element.relativeRect.x = Mathf.RoundToInt( element.relativeRect.x );
		element.relativeRect.y = Mathf.RoundToInt( element.relativeRect.y );
		element.relativeRect.width = Mathf.RoundToInt( element.relativeRect.width );
		element.relativeRect.height = Mathf.RoundToInt( element.relativeRect.height );
	}

	#endregion

	#region Context Menu

	void ContextMenuFunction ( int windowID ) {
		GUI.Box( new Rect( 0, 0, contextRect.width, contextRect.height ), "" );

		if ( chosenGUIPart != TypesOfGUIElement.SelectMany || currentSelectedElements.Count == 1 ) {
			GUI.skin = editorSkin;
			GUI.Label( new Rect( 3, 3, 95, 20 ), "" + typesOfGUIElement[currentSelectedElements[0].partID], "Small Label" );
			GUI.skin = null;
		}

		GUI.SetNextControlName( "Element Color" );
		try {
			contextColor = EditorGUI.ColorField( new Rect( 100, 3, 45, 20 ), contextColor );
		} catch { }
		if ( contextColor != contextColorOld ) {
			for ( int i = 0; i < currentSelectedElements.Count; i++ ) {
				currentSelectedElements[i].partColor = contextColor;
			}
			contextColorOld = contextColor;
		}


		if ( chosenGUIPart != TypesOfGUIElement.SelectMany || currentSelectedElements.Count == 1 ) {
			GUI.skin.toggle.fontSize = 9;
			currentSelectedElements[0].iAmList = GUI.Toggle( new Rect( 99, 23, 70, 15 ), currentSelectedElements[0].iAmList, "List" );
			GUI.skin.toggle.fontSize = 0;

			GUI.enabled = GUIElementsAllowLabel[currentSelectedElements[0].partID];
			GUI.Label( new Rect( 3, 23, 140, 20 ), "Label:" );
			GUI.SetNextControlName( "Element Label" );
			if ( GUIElementsAllowLabel[currentSelectedElements[0].partID] ) {
				currentSelectedElements[0].label = EditorGUI.TextArea( new Rect( 3, 40, 140, 20 ), currentSelectedElements[0].label );
			} else {
				EditorGUI.TextArea( new Rect( 3, 40, 140, 20 ), "" );
			}

			GUI.enabled = GUIElementsVariableType[currentSelectedElements[0].partID] != "None";
			GUI.Label( new Rect( 3, 63, 140, 20 ), "Variable (" + GUIElementsVariableType[currentSelectedElements[0].partID] + "):" );
			GUI.SetNextControlName( "Element Variable" );
			currentSelectedElements[0].variableName = EditorGUI.TextArea( new Rect( 3, 80, 140, 20 ), currentSelectedElements[0].variableName );

			GUI.enabled = true;
		}

		GUI.Label( new Rect( 80, 108, 70, 20 ), "Anchor:" );
		GUI.SetNextControlName( "Element Anchor" );
		contextAnchor = (AnchorLocation) EditorGUI.EnumPopup( new Rect( 76, 125, 70, 20 ), contextAnchor );
		if ( contextAnchor != contextAnchorOld ) {
			for ( int i = 0; i < currentSelectedElements.Count; i++ ) {
				currentSelectedElements[i].anchoredTo = contextAnchor;
			}
			contextAnchorOld = contextAnchor;
		}

		if ( chosenGUIPart != TypesOfGUIElement.SelectMany || currentSelectedElements.Count == 1 ) {
			GUI.skin.button.fontSize = 8;
			int depth = currentSelectedElements[0].containingElement.groupedElements.IndexOf( currentSelectedElements[0] );
			GUI.enabled = depth != currentSelectedElements[0].containingElement.groupedElements.Count - 1;
			if ( GUI.Button( new Rect( 3, 104, 35, 19 ), "Up" ) ) {
				currentSelectedElements[0].containingElement.groupedElements[depth] = currentSelectedElements[0].containingElement.groupedElements[depth + 1];
				currentSelectedElements[0].containingElement.groupedElements[depth + 1] = currentSelectedElements[0];
				ReassesAllElements();
			}
			GUI.enabled = depth != 0;
			if ( GUI.Button( new Rect( 3, 125, 35, 19 ), "Down" ) ) {
				currentSelectedElements[0].containingElement.groupedElements[depth] = currentSelectedElements[0].containingElement.groupedElements[depth - 1];
				currentSelectedElements[0].containingElement.groupedElements[depth - 1] = currentSelectedElements[0];
				ReassesAllElements();
			}
			GUI.enabled = depth != currentSelectedElements[0].containingElement.groupedElements.Count - 1;
			if ( GUI.Button( new Rect( 39, 104, 35, 19 ), "Front" ) ) {
				currentSelectedElements[0].containingElement.groupedElements.Remove( currentSelectedElements[0] );
				currentSelectedElements[0].containingElement.groupedElements.Add( currentSelectedElements[0] );
				ReassesAllElements();
			}
			GUI.enabled = depth != 0;
			if ( GUI.Button( new Rect( 39, 125, 35, 19 ), "Back" ) ) {
				currentSelectedElements[0].containingElement.groupedElements.Remove( currentSelectedElements[0] );
				currentSelectedElements[0].containingElement.groupedElements.Insert( 0, currentSelectedElements[0] );
				ReassesAllElements();
			}
			GUI.enabled = true;
			GUI.skin.button.fontSize = 0;
		}


		if ( GUI.Button( new Rect( 3, 165, 71, 19 ), "Re-Group" ) ) {
			insertingToGroup = true;
			HideContext();
		}

		GUI.skin.button.fontSize = 9;
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		GUI.SetNextControlName( "Element Re-Type" );
		contextGUIPart = EditorGUI.Popup( new Rect( 76, 165, 71, 19 ), contextGUIPart, contextTypesLabels, "Button" );
		if ( contextGUIPart != 0 ) {
			foreach ( GUIElement element in currentSelectedElements ) {
				element.partID = contextGUIPart;
				element.myGUI = GUIDelegates[element.partID];
				element.myName = shortNamesOfGUIElement[element.partID];
				if ( defailtVariableNames[element.partID] == "" ) element.variableName = "";
				if ( !GUIElementsAllowLabel[element.partID] ) {
					element.label = "";
				} else if ( element.label == "" || element.label == defaultLabels[element.oldID] ) {
					element.label = defaultLabels[element.partID];
				}
				if ( element.partID == TypesOfGUIElement.EditorGUIFoldout || element.partID == TypesOfGUIElement.GUIToggle ) element.elementSpecificBool = true;
				if ( element.partID == TypesOfGUIElement.GUIGroup || element.partID == TypesOfGUIElement.GUIScrollview ) element.elementSpecificBool = false;
				if ( GUIElementsCanContainOthers[element.oldID] && !GUIElementsCanContainOthers[element.partID] ) {
					foreach ( GUIElement child in element.groupedElements ) {
						allElements.Remove( child );
					}
					element.groupedElements.Clear();
				}
				element.oldID = element.partID;
			}
			ReassesAllElements();
			contextGUIPart = 0;
		}

		GUI.skin.button.fontSize = 0;
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;


		if ( chosenGUIPart == TypesOfGUIElement.SelectMany ) {
			if ( AllOfType( TypesOfGUIElement.GUIGroup, TypesOfGUIElement.GUIScrollview ) ) {
				groupElementSpecificBool = GUI.Toggle( new Rect( 3, 184, 140, 19 ), groupElementSpecificBool, "Fill with box" );
				if ( groupElementSpecificBool != groupElementSpecificBoolOld ) {
					groupElementSpecificBoolOld = groupElementSpecificBool;
					foreach ( GUIElement element in currentSelectedElements ) {
						element.elementSpecificBool = groupElementSpecificBool;
					}
				}
			}

			if ( AllOfType( TypesOfGUIElement.GUISelectionGrid ) ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 20;
#else
		EditorGUIUtility.LookLikeControls( 20 );
#endif
				groupElementSpecficInt1 = Mathf.Max( EditorGUI.IntField( new Rect( 3, 185, 71, 16 ), "C:", groupElementSpecficInt1 ), 2 );
				groupElementSpecficInt2 = Mathf.Max( EditorGUI.IntField( new Rect( 76, 185, 71, 16 ), "X:", groupElementSpecficInt2 ), 1 );
				if ( groupElementSpecficInt1 != groupElementSpecficInt1Old || groupElementSpecficInt2 != groupElementSpecficInt2Old ) {
					groupElementSpecficInt1Old = groupElementSpecficInt1;
					groupElementSpecficInt2Old = groupElementSpecficInt2;
					foreach ( GUIElement element in currentSelectedElements ) {
						element.elementSpecificInt1 = groupElementSpecficInt1;
						element.elementSpecificInt2 = groupElementSpecficInt2;
					}
				}

			}

			if ( AllOfType( TypesOfGUIElement.EditorGUIColorField, TypesOfGUIElement.EditorGUIFloatField, TypesOfGUIElement.EditorGUIIntField, TypesOfGUIElement.EditorGUIEnumPopup, TypesOfGUIElement.EditorGUIObjectField ) ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 80;
#else
		EditorGUIUtility.LookLikeControls( 80 );
#endif
				groupElementSpecificBool = GUI.Toggle( new Rect( 3, 184, 20, 19 ), groupElementSpecificBool, "" );
				if ( groupElementSpecificBool != groupElementSpecificBoolOld ) {
					groupElementSpecificBoolOld = groupElementSpecificBool;
					foreach ( GUIElement element in currentSelectedElements ) {
						element.elementSpecificBool = groupElementSpecificBool;
					}
				}
				GUI.enabled = groupElementSpecificBool;
				groupElementSpecficInt1 = Mathf.Max( EditorGUI.IntField( new Rect( 20, 185, 110, 16 ), "Label Width:", groupElementSpecficInt1 ), 2 );
				GUI.enabled = true;
				if ( groupElementSpecficInt1 != groupElementSpecficInt1Old ) {
					groupElementSpecficInt1Old = groupElementSpecficInt1;
					foreach ( GUIElement element in currentSelectedElements ) {
						element.elementSpecificInt1 = groupElementSpecficInt1;
					}
				}
			}

			if ( AllOfType( TypesOfGUIElement.EditorGUIVectorField ) ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 50;
#else
		EditorGUIUtility.LookLikeControls( 50 );
#endif
				groupElementSpecficInt1 = Mathf.Clamp( EditorGUI.IntField( new Rect( 3, 185, 70, 16 ), "Vector", groupElementSpecficInt1 ), 2, 4 );
				if ( groupElementSpecficInt1 != groupElementSpecficInt1Old ) {
					groupElementSpecficInt1Old = groupElementSpecficInt1;
					foreach ( GUIElement element in currentSelectedElements ) {
						element.elementSpecificInt1 = groupElementSpecficInt1;
					}
				}
			}

			if ( AllOfType( TypesOfGUIElement.GUISliderHorizontal, TypesOfGUIElement.GUISliderVertical, TypesOfGUIElement.GUIScrollbarHorizontal, TypesOfGUIElement.GUIScrollbarVertical ) ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 30;
#else
		EditorGUIUtility.LookLikeControls( 30 );
#endif
				groupElementSpecficFloat1 = Mathf.Min( EditorGUI.FloatField( new Rect( 3, 185, 70, 16 ), "Min", groupElementSpecficFloat1 ), groupElementSpecficFloat2 );
				groupElementSpecficFloat2 = Mathf.Max( EditorGUI.FloatField( new Rect( 76, 185, 70, 16 ), "Max", groupElementSpecficFloat2 ), groupElementSpecficFloat1 );
				if ( groupElementSpecficFloat1 != groupElementSpecficFloat1Old || groupElementSpecficFloat2 != groupElementSpecficFloat2Old ) {
					groupElementSpecficFloat1Old = groupElementSpecficFloat1;
					groupElementSpecficFloat2Old = groupElementSpecficFloat2;
					foreach ( GUIElement element in currentSelectedElements ) {
						element.elementSpecificFloat1 = groupElementSpecficFloat1;
						element.elementSpecificFloat2 = groupElementSpecficFloat2;
					}
				}
			}
		} else {

			if ( currentSelectedElements[0].partID == TypesOfGUIElement.GUIGroup || currentSelectedElements[0].partID == TypesOfGUIElement.GUIScrollview ) {
				currentSelectedElements[0].elementSpecificBool = GUI.Toggle( new Rect( 3, 184, 140, 19 ), currentSelectedElements[0].elementSpecificBool, "Fill with box" );
			}

			if ( currentSelectedElements[0].partID == TypesOfGUIElement.GUISelectionGrid ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 20;
#else
		EditorGUIUtility.LookLikeControls( 20 );
#endif
				currentSelectedElements[0].elementSpecificInt1 = Mathf.Max( EditorGUI.IntField( new Rect( 3, 185, 71, 16 ), "C:", currentSelectedElements[0].elementSpecificInt1 ), 2 );
				currentSelectedElements[0].elementSpecificInt2 = Mathf.Max( EditorGUI.IntField( new Rect( 76, 185, 71, 16 ), "X:", currentSelectedElements[0].elementSpecificInt2 ), 1 );

			}

			if ( currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIColorField ||
				currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIFloatField ||
				currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIIntField ||
				currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIEnumPopup ||
				currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIObjectField ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 80;
#else
		EditorGUIUtility.LookLikeControls( 80 );
#endif
				currentSelectedElements[0].elementSpecificBool = GUI.Toggle( new Rect( 3, 184, 20, 19 ), currentSelectedElements[0].elementSpecificBool, "" );
				GUI.enabled = currentSelectedElements[0].elementSpecificBool;
				currentSelectedElements[0].elementSpecificInt1 = Mathf.Max( EditorGUI.IntField( new Rect( 20, 185, 110, 16 ), "Label Width:", currentSelectedElements[0].elementSpecificInt1 ), 2 );
				GUI.enabled = true;
			}

			if ( currentSelectedElements[0].partID == TypesOfGUIElement.EditorGUIVectorField ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 50;
#else
		EditorGUIUtility.LookLikeControls( 50 );
#endif
				currentSelectedElements[0].elementSpecificInt1 = Mathf.Clamp( EditorGUI.IntField( new Rect( 3, 185, 70, 16 ), "Vector", currentSelectedElements[0].elementSpecificInt1 ), 2, 4 );
			}

			if ( currentSelectedElements[0].partID == TypesOfGUIElement.GUISliderHorizontal ||
				currentSelectedElements[0].partID == TypesOfGUIElement.GUISliderVertical ||
				currentSelectedElements[0].partID == TypesOfGUIElement.GUIScrollbarHorizontal ||
				currentSelectedElements[0].partID == TypesOfGUIElement.GUIScrollbarVertical ) {
#if UNITY_4_3
				EditorGUIUtility.labelWidth = 30;
#else
		EditorGUIUtility.LookLikeControls( 30 );
#endif
				currentSelectedElements[0].elementSpecificFloat1 = Mathf.Min( EditorGUI.FloatField( new Rect( 3, 185, 70, 16 ), "Min", currentSelectedElements[0].elementSpecificFloat1 ), currentSelectedElements[0].elementSpecificFloat2 );
				currentSelectedElements[0].elementSpecificFloat2 = Mathf.Max( EditorGUI.FloatField( new Rect( 76, 185, 70, 16 ), "Max", currentSelectedElements[0].elementSpecificFloat2 ), currentSelectedElements[0].elementSpecificFloat1 );
			}
		}

		// These need to go last in the context block becuase they render things null which the block relies on
		if ( GUI.Button( new Rect( 3, 145, 71, 19 ), "Delete" ) ) {
			DeleteElements();
		}
		if ( GUI.Button( new Rect( 76, 145, 71, 19 ), "Copy" ) ) {
			CopyElements( true );
		}

		GUI.DragWindow();
	}
	#endregion

	#region Help Window

	void HelpMenuFunction ( int windowID ) {
		GUI.skin = editorSkin;
		GUI.skin = editorSkin;
		GUI.Label( new Rect( 0, 20, helpWindowRect.width, 64 ), "GUIMaker by hpjohn", "Large Label" );
		GUI.Label( new Rect( 0, 37, helpWindowRect.width, 64 ), "A WYSIWIG editor for UnityGUI layouts.\n", "Mid Label" );
		GUI.Label( new Rect( 460, 22, 60, 40 ), "Click to\nclose!", "Mid Label" );
		GUI.Box( new Rect( 4, 55, 512, 512 ), "", "Help" );
		GUI.skin = null;
		if ( GUI.Button( new Rect( 0, 0, helpWindowRect.width, helpWindowRect.height ), "", "" ) ) {
			ShowHelpDialog = false;
		}

		GUI.DragWindow();


	}

	#endregion

	#region Save and Load

	void DoSave () {
		result = "";
		tabCount = 0;
		listCount = -1;
		currentColor = Color.white;
		userVariableCounts = new int[GUIElementsVariableTypeCased.Length];
		userVariableDeclarations = new List<string>();
		userVariables = new List<string>();
		resultLines = new List<string>();
		warnings = new List<string>();
		windowDeclarations = new List<string>();
		allWindows = new List<List<string>>();
		currentEditorLabelSize = 0;
		usesEditors = false;
		RecursiveComplete( screenElement );

		string errorList = "";
		if ( warnings.Count != 0 ) {
			errorList += "//Errors:\n";
			foreach ( string s in warnings ) {
				errorList += s;
			}
			errorList += "\n\n";
		}

		string headers = "";
		if ( usesEditors ) {
			if ( inCSharp ) {
				headers +=
				"using UnityEngine;\n" +
				"using UnityEditor;\n" +
				"using System.Collections;\n" +
				"using System.Collections.Generic;\n\n" +
				"//Remember to change every SCRIPTNAME to the name of the script\n" +
				"public class SCRIPTNAME : EditorWindow {\n\n" +
				"[MenuItem( \"Window/SCRIPTNAME\" )]\n" +
				"static void Init () {\n" +
				"SCRIPTNAME window = (SCRIPTNAME) EditorWindow.GetWindow( typeof( SCRIPTNAME ) );\n" +
				"\twindow.Show();\n" +
				"\twindow.position = new Rect( 20, 80, " + screenSizeX + ", " + screenSizeY + " );\n" +
				"}\n";
			} else {
				headers +=
				"#pragma strict\n" +
				"import System.Collections.Generic;\n\n" +
				"//Remember to change every SCRIPTNAME to the name of the script\n" +
				"class SCRIPTNAME extends EditorWindow {\n\n" +
				"@MenuItem (\"Window/SCRIPTNAME\")\n" +
				"static function Initialize() {\n" +
				"\tvar window = ScriptableObject.CreateInstance.<SCRIPTNAME>();\n" +
				"\twindow.Show();\n" +
				"\twindow.position = Rect( 20, 80, " + screenSizeX + ", " + screenSizeY + " );\n" +
				"}\n";
			}
		} else {
			if ( inCSharp ) {
				headers +=
				"using UnityEngine;\n" +
				"using System.Collections;\n" +
				"using System.Collections.Generic;\n\n" +
				"//Remember to change SCRIPTNAME to the name of the script\n" +
				"public class SCRIPTNAME : MonoBehaviour {\n\n";
			} else {
				headers +=
				"#pragma strict\n" +
				"import System.Collections.Generic;\n\n";
			}
		}

		string variableList = "//Required Variables:\n";
		if ( inCSharp ) {
			if ( previewSkin != null ) variableList += ( usesEditors ? "" : "public " ) + "GUISkin someGUISkin;\n";
		} else {
			if ( previewSkin != null ) variableList += "var someGUISkin : GUISkin;\n";
		}
		foreach ( string s in userVariableDeclarations ) {
			variableList += s;
		}
		variableList += "\n\n";
		if ( !inCSharp ) {
			variableList = variableList.Replace( "bool", "boolean" );
			variableList = variableList.Replace( " string ", " String " );
		}

		string collectedLines = "";
		if ( inCSharp ) {
			collectedLines += "void OnGUI () {\n";
		} else {
			collectedLines += "function OnGUI () {\n";
		}
		if ( previewSkin != null ) collectedLines += "GUI.skin = someGUISkin;\n";
		foreach ( string s in resultLines ) {
			collectedLines += s;
		}

		if ( allWindows.Count != 0 ) {
			collectedLines += "//If this is an Editor GUI, you will need to uncomment this line:\n";
			collectedLines += "//BeginWindows();\n";

			foreach ( string s in windowDeclarations ) {
				collectedLines += s + "\n";
			}

			collectedLines += "//If this is an Editor GUI, you will need to uncomment this line:\n";
			collectedLines += "//EndWindows();\n";
		}
		collectedLines += "}\n\n";

		string windowLines = "";
		if ( allWindows.Count != 0 ) {
			windowLines += "//Here are the window functions:\n";

			for ( int i = 0; i < allWindows.Count; i++ ) {
				if ( inCSharp ) {
					windowLines += "void WindowFunction" + i + " ( int windowID ) {\n";
				} else {
					windowLines += "function WindowFunction" + i + " (windowID : int) {\n";
				}
				foreach ( string s in allWindows[i] ) {
					windowLines += s;
				}
				windowLines += "\tGUI.DragWindow();\n";
				if ( inCSharp || usesEditors ) windowLines += "}\n\n";
			}
		}

		result = ( IncludeHeaders ? headers : "" ) + errorList + variableList + collectedLines + windowLines + ( IncludeHeaders ? "}" : "" ) + "\n\n";

		GenerateSaveCode();

		result += saveCode;

		EditorGUIUtility.systemCopyBuffer = result;

	}

	void ParseLoadCode ( string loadCode ) {
		if ( loadCode.IndexOf( "GUIMaker" ) == -1 ) {
			Debug.Log( "Bad/No code on the clipboard, copy from the first '//GUIMaker' to the last" );
			return;
		}
		if ( loadCode.Split( new string[] { "GUIMaker" }, System.StringSplitOptions.None ).Length != 3 ) {
			Debug.Log( "Bad code on the clipboard, copy from the first '//GUIMaker' to the last" );
			return;
		}

		loadCode = loadCode.Split( new string[] { "GUIMaker" }, System.StringSplitOptions.None )[1];
		string[] split = loadCode.Split( '\n' );

		if ( split.Length != 14 ) {
			Debug.Log( "Bad code on the clipboard, including the first and last 'GUIMaker' it should only be 14 lines" );
			return;
		}

		string[][] allProperties = new string[split.Length][];
		for ( int i = 0; i < split.Length; i++ ) {
			allProperties[i] = split[i].Split( ';' );
		}

		for ( int i = 0; i < allProperties[1].Length - 2; i++ ) {
			GUIElement newElement = new GUIElement();
			newElement.oldID = newElement.partID = int.Parse( allProperties[1][i] );
			allElements.Add( newElement );
		}
		for ( int i = 0; i < allElements.Count; i++ ) {
			allElements[i].oldID = allElements[i].partID = int.Parse( allProperties[1][i] );
			allElements[i].containingElement = int.Parse( allProperties[2][i] ) == -1 ? null : allElements[int.Parse( allProperties[2][i] )];
			allElements[i].relativeRect.x = float.Parse( allProperties[3][i].Split( ',' )[0] );
			allElements[i].relativeRect.y = float.Parse( allProperties[3][i].Split( ',' )[1] );
			allElements[i].relativeRect.width = float.Parse( allProperties[3][i].Split( ',' )[2] );
			allElements[i].relativeRect.height = float.Parse( allProperties[3][i].Split( ',' )[3] );
			allElements[i].label = allProperties[4][i];
			allElements[i].variableName = allProperties[5][i];
			allElements[i].iAmList = int.Parse( allProperties[6][i] ) == 1;
			allElements[i].anchoredTo = (AnchorLocation) int.Parse( allProperties[7][i] );
			allElements[i].listOffset.x = float.Parse( allProperties[8][i].Split( ',' )[0] );
			allElements[i].listOffset.y = float.Parse( allProperties[8][i].Split( ',' )[1] );
			allElements[i].scrollOffset.x = float.Parse( allProperties[9][i].Split( ',' )[0] );
			allElements[i].scrollOffset.y = float.Parse( allProperties[9][i].Split( ',' )[1] );
			allElements[i].partColor.r = float.Parse( allProperties[10][i].Split( ',' )[0] );
			allElements[i].partColor.g = float.Parse( allProperties[10][i].Split( ',' )[1] );
			allElements[i].partColor.b = float.Parse( allProperties[10][i].Split( ',' )[2] );
			allElements[i].partColor.a = float.Parse( allProperties[10][i].Split( ',' )[3] );
			allElements[i].elementSpecificInt1 = int.Parse( allProperties[11][i].Split( ',' )[0] );
			allElements[i].elementSpecificInt2 = int.Parse( allProperties[11][i].Split( ',' )[1] );
			allElements[i].elementSpecificBool = int.Parse( allProperties[11][i].Split( ',' )[2] ) == 1;
			allElements[i].myName = shortNamesOfGUIElement[allElements[i].partID];

			string[] group = allProperties[12][i].Split( ',' );
			foreach ( string s in group ) {
				int parsed = 0;
				if ( int.TryParse( s, out parsed ) ) {
					allElements[i].groupedElements.Add( allElements[parsed] );
				}
			}
			allElements[i].myName = shortNamesOfGUIElement[allElements[i].partID];
			allElements[i].myGUI = GUIDelegates[allElements[i].partID];
		}
		ReassesAllElements();
	}



	void GenerateSaveCode () {
		string fullSaveCode = "/*GUIMaker by hpjohn. Savecode:";

		string stringIDs = "\n";
		string stringContainers = "\n";
		string stringRects = "\n";
		string stringLabels = "\n";
		string stringVariables = "\n";
		string stringLists = "\n";
		string stringAnchors = "\n";
		string stringListOffsets = "\n";
		string stringScrollOffsets = "\n";
		string stringColors = "\n";
		string stringSpecifics = "\n";
		string stringGroupContents = "\n";

		for ( int i = 0; i < allElements.Count; i++ ) {
			GUIElement element = allElements[i];
			stringIDs += element.partID + ";";
			stringContainers += allElements.IndexOf( element.containingElement ) + ";";
			stringRects += element.relativeRect.x + "," + element.relativeRect.y + "," + element.relativeRect.width + "," + element.relativeRect.height + ";";
			//stringLabelss += ( element.label == defaultLabels[element.partID] ? "" : element.label ) + ";";
			stringLabels += element.label + ";";
			//stringVariables += ( element.variableName == defailtVariableNames[element.partID] ? "" : element.variableName ) + ";";
			stringVariables += element.variableName + ";";
			stringLists += ( element.iAmList ? 1 : 0 ) + ";";
			stringAnchors += (int) element.anchoredTo + ";";
			stringListOffsets += element.listOffset.x + "," + element.listOffset.y + ";";
			stringScrollOffsets += element.scrollOffset.x + "," + element.scrollOffset.y + ";";
			stringColors += element.partColor.r + "," + element.partColor.g + "," + element.partColor.b + "," + element.partColor.a + ";";
			stringSpecifics += element.elementSpecificInt1 + ",";
			stringSpecifics += element.elementSpecificInt2 + ",";
			stringSpecifics += ( element.elementSpecificBool ? 1 : 0 ) + ";";

			for ( int j = 0; j < element.groupedElements.Count; j++ ) {
				stringGroupContents += allElements.IndexOf( element.groupedElements[j] ) + ( j == element.groupedElements.Count - 1 ? "" : "," );
			}
			stringGroupContents += ";";


		}

		screenSizeX = (int) allElements[0].relativeRect.width;
		screenSizeY = (int) allElements[0].relativeRect.height;

		fullSaveCode += stringIDs + stringContainers + stringRects + stringLabels + stringVariables + stringLists + stringAnchors + stringListOffsets + stringScrollOffsets + stringColors + stringSpecifics + stringGroupContents;
		saveCode = fullSaveCode + "\nGUIMaker*/\n";


	}

	#endregion
}
