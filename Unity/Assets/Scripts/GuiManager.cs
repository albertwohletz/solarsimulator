using UnityEngine;
using System.Collections;
using System;

enum DescriptionType{
	DEFAULT,
	COMPOSITION,
	HISTORY
};

public class GuiManager : MonoBehaviour {
	string focus = "";
	MoveCamera cam;
	private GUIStyle currentStyle = null;
	private DescriptionType type = DescriptionType.DEFAULT;

	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<MoveCamera> (); 
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI () 
	{       
		currentStyle = new GUIStyle( GUI.skin.box );
		currentStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 0.5f ) );

		// Make a background box
		DrawDescriptionBox ();
		if(GUI.Button(new Rect(20,0,80,20), "Sun")) {
			focus = "Sun";
			cam.SetFocus("Sun");
		}
		
		if(GUI.Button(new Rect(20,30,80,20), "Mercury")) {
			focus = "Mercury";
			cam.SetFocus("Mercury");
		}
		
		if(GUI.Button(new Rect(20,60,80,20), "Venus")) {
			focus = "Venus";
			cam.SetFocus("Venus");
		}
		
		if(GUI.Button(new Rect(20,90,80,20), "Earth")) {
			focus = "Earth";
			cam.SetFocus("Earth");
		} 
		if(GUI.Button(new Rect(20,120,80,20), "Mars")) {
			focus = "Mars";
			cam.SetFocus("Mars");
		} 
		if(GUI.Button(new Rect(20,150,80,20), "Jupiter")) {
			focus = "Jupiter";
			cam.SetFocus("Jupiter");
		} 
		if(GUI.Button(new Rect(20,180,80,20), "Saturn")) {
			focus = "Saturn";
			cam.SetFocus("Saturn");
		} 
		if(GUI.Button(new Rect(20,210,80,20), "Uranus")) {
			focus = "Uranus";
			cam.SetFocus("Uranus");
		} 
		if(GUI.Button(new Rect(20,240,80,20), "Neptune")) {
			focus = "Neptune";
			cam.SetFocus("Neptune");
		} 

	}

	void DrawDescriptionBox(){
		string focusstr = "";
		if(focus == "Earth"){
			if (type == DescriptionType.DEFAULT){
				focusstr = "Population: 7.046 billion humans\n" + 
							"Age: 4.54 billion years\n" +
							"Mass: 5.972E24 kg\n" +
							"Distance from Sun: 149,600,000 km (92,960,000 miles)\n"+
							"Mean radius: 6371.0 km\n" +
							"Year: 365.26 days\n"+
							"Day: 24 Hours";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "Atmosphere:\n" +
					"78.08% nitrogen (N2)\n" +
					"20.95% oxygen (O2)\n" +
					"0.930% argon\n" + 
					"0.039% carbon dioxide\n" +
					"1% water vapor (climate-variable)";
			}
		} else if(focus == "Sun"){
			if (type == DescriptionType.DEFAULT){
				focusstr = "Mass: 1.98855E30 kg, 333,000 Earths.\n " +
						   "Surface area: 6.09E12 km^2 12000 × Earth.\n" + 
						   "Age: 4.6 billion years.\n" +
						   "Remaining Fuel: 5 billion years.";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "Hydrogen:	73.46%\n"+
							"Helium:    24.85%\n"+
							"Oxygen:    0.77%\n"+
							"Carbon:    0.29%\n"+
							"Iron:	    0.16%\n"+
							"Neon:	    0.12%\n"+
							"Nitrogen:	0.09%\n"+
							"Silicon:	0.07%\n"+
							"Magnesium:	0.05%\n"+
							"Sulfur:	0.04%\n";
			}
		} else if(focus == "Mercury"){
				if (type == DescriptionType.DEFAULT){
					focusstr = "Mercury is the smallest and closest to the Sun of the eight planets in the Solar System.\n" +
							"Density: 5.43 g/cm³\n"+
							"Radius: 1,516 miles (2,440 km)\n"+
							"Surface area: 28.88 million sq miles (74.8 million km²)\n"+
							"Mass: 328.5E21 kg (0.055 Earth mass)\n"+
							"Distance from Sun: 35,980,000 miles (57,910,000 km)\n"+
							"Length of year: 88d\n"+
							"Length of day: 58d 15h 30m ";

				} else if (type == DescriptionType.COMPOSITION){
					
				}
		} else if(focus == "Venus"){
			if (type == DescriptionType.DEFAULT){
				
			} else if (type == DescriptionType.COMPOSITION){
				
			}
		} else if(focus == "Mars"){
			if (type == DescriptionType.DEFAULT){
				
			} else if (type == DescriptionType.COMPOSITION){
				
			}
		} else if(focus == "Saturn"){
			if (type == DescriptionType.DEFAULT){
				
			} else if (type == DescriptionType.COMPOSITION){
				
			}
		} else if(focus == "Jupiter"){
			if (type == DescriptionType.DEFAULT){
				
			} else if (type == DescriptionType.COMPOSITION){
				
			}
		} else if(focus == "Uranus"){
			if (type == DescriptionType.DEFAULT){
				
			} else if (type == DescriptionType.COMPOSITION){
				
			}
		} else if(focus == "Neptune"){
			if (type == DescriptionType.DEFAULT){
				
			} else if (type == DescriptionType.COMPOSITION){
				
			}
		}

		// Description Box
		int boxheight = 200;
		GUI.Box (new Rect (10, Screen.height - boxheight - 40, 500, boxheight), focusstr);

		// Buttons
		if (GUI.Button (new Rect (20,Screen.height - 30,100,30), "Description")) {
			type = DescriptionType.DEFAULT;
		} 
		if (GUI.Button (new Rect (130,Screen.height - 30,100,30), "Composition")) {
			type = DescriptionType.COMPOSITION;
		} 

		// Date Box
		GUI.Box (new Rect (Screen.width/2 - 100,Screen.height - 30,200,30), GetDate ());
	}

	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	const float days_per_second = 2.60714285714286f;
	private string GetDate(){
		float seconds = days_per_second * Time.time * (60 * 60 * 24);

		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new System.DateTime(2000,1,1,0,0,0,0,System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds (seconds).ToLocalTime();
		return dtDateTime.ToString () + " " + (days_per_second * Time.time).ToString ();
	}
}
