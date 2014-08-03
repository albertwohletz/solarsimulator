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
				focusstr = "Earth is the only known planet to support life.\n"+
							"Population: 7.046 billion humans\n" + 
							"Age: 4.54 billion years\n" +
							"Mass: 5.972E24 kg\n" +
							"Distance from Sun: 149,600,000 km (92,960,000 miles)\n"+
							"Mean radius: 6371.0 km\n" +
							"Year: 365.26 days\n"+
							"Day: 24 Hours\n"+
							"Avg Temp: 59 F° (15 C°)\n" +
							"Max Temp: 158 F° (70 C°)\n" +
							"Min Temp: -128 F° (-89 C°)\n";
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
						   "Remaining Fuel: 5 billion years.\n"+
						"Avg Temp of Photosphere: 9941 F° (5505 C°)";
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
							"Length of day: 58d 15h 30m\n"+
							"Avg Temp: 332 F° (167 C°)\n" +
							"Max Temp: 801 F° (427 C°)\n" +
							"Min Temp: -79 F° (-173 C°)\n";

				} else if (type == DescriptionType.COMPOSITION){
						focusstr = "42% Molecular oxygen\n" +
						"29.0% sodium\n" +
						"22.0% hydrogen\n" +
						"6.0% helium\n" +
						"0.5% potassium\n" +
						"Trace amounts of argon, nitrogen, carbon dioxide, water vapor, xenon, krypton, and neon.";
				}
		} else if(focus == "Venus"){
			if (type == DescriptionType.DEFAULT){
				focusstr = "Venus has an extremely thick CO2-rich atmosphere.  \n" +
					"Radius: 3,760 miles (6,052 km)\n" +
					"Surface area: 177.7 million sq miles (460.2 million km²)\n" +
					"Mass: 4.867E24 kg (0.815 Earth mass)\n" +
					"Distance from Sun: 67,240,000 miles (108,200,000 km)\n"+
					"Length of day: 116 days 18 hours 0 minutes\n"+
						"Length of year: 225 days\n" +
						"Avg Temp: 867 °F (463 °C)\n"+
						"Max Temp: 870 °F (465 °C)\n"+
						"Min Temp: 864 °F (462 °C)\n";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "96.5% carbon dioxide\n" +
					"3.5% nitrogen\n" +
						"0.015% sulfur dioxide\n" +
						"0.007% argon\n" +
						"0.002% water vapour\n" +
						"0.0017% carbon monoxide\n" +
						"0.0012% helium\n" +
						"0.0007% neon\n" +
						"trace carbonyl sulfide\n" +
						"trace hydrogen chloride\n" +
						"trace hydrogen fluoride\n";
			}
		} else if(focus == "Mars"){
			if (type == DescriptionType.DEFAULT){
					focusstr = "Radius: 2,106 miles (3,390 km)\n"+
					"Mass: 639E21 kg (0.107 Earth mass)\n"+
					"Surface area: 55.91 million sq miles (144.8 million km²)\n"+
					"Length of day: 1 day 40 minutes\n"+
					"Length of year: 687 days\n"+
					"Avg Temp: -81 °F (-63 °C)\n"+
					"Max Temp: -287 °F (-177 °C)\n"+
					"Min Temp: 86 °F (-287 °C)";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "95.97% carbon dioxide\n"+
				"1.93% argon\n"+
				"1.89% nitrogen\n"+
				"0.146% oxygen\n"+
				"0.0557% carbon monoxide\n"+
				"210 ppm water vapor\n"+
				"100 ppm nitric oxide\n"+
				"15 ppm molecular hydrogen\n"+
				"2.5 ppm neon\n"+
				"850 ppb HDO\n"+
				"300 ppb krypton\n"+
				"130 ppb formaldehyde\n"+
				"80 ppb xenon\n"+
				"18 ppb hydrogen peroxide\n"+
				"10 ppb methane\n";
			}
		} else if(focus == "Saturn"){
			if (type == DescriptionType.DEFAULT){
				focusstr = "Radius: 36,184 miles (58,232 km)\n"+
				"Mass: 568.3E24 kg (95.16 Earth mass)\n"+
				"Surface area: 16.49 billion sq miles (42.7 billion km²)\n"+
				"Distance from Sun: 890,700,000 miles (1,433,000,000 km)\n"+
				"Length of day: 10 hours 39 minutes\n" + 
				"Length of year: 29 years.\n"+
				"Avg Surface Temp: -288 °F (-178 °C)\n";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "96%	hydrogen (H2)\n" + 
				"3%	helium (He)\n" + 
				"0.4%	methane (CH4)\n" + 
				"0.01%	ammonia (NH3)\n" + 
				"0.01%	hydrogen deuteride (HD)\n" + 
				"0.0007%	ethane (C2H6)";
			}
		} else if(focus == "Jupiter"){
			if (type == DescriptionType.DEFAULT){
				focusstr = "Radius: 43,441 miles (69,911 km)\n" +
							"Mass: 1.898E27 kg (317.8 Earth mass)\n" +
							"Surface area: 23.71 billion sq miles (61.42 billion km²)\n" +
							"Distance from Sun: 483,800,000 miles (778,500,000 km)\n" +
							"Length of day: 9 hours 56 minutes\n" +
						"Length of year: 12 years.\n"+
						"Avg Surface Temp: -234 °F (-145 °C)";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "89.8%	hydrogen (H2)\n" +
				"10.2%	helium (He)\n" +
				"0.3%	methane (CH4)\n" +
				"0.026%	ammonia (NH3)\n" +
				"0.003%	hydrogen deuteride (HD)\n" +
				"0.0006%	ethane (C2H6)\n" +
				"0.0004%	water (H2O)";
			}
		} else if(focus == "Uranus"){
			if (type == DescriptionType.DEFAULT){
				focusstr = "Radius: 15,759 miles (25,362 km)\n" +
						"Mass: 86.81E24 kg (14.54 Earth mass)\n" +
						"Surface area: 3.121 billion sq miles (8.083 billion km²)\n"+
						"Distance from Sun: 1,787,000,000 miles (2,877,000,000 km)\n"+
						"Length of day: 17 hours 14 minutes\n"+
						"Length of year: 84 years\n"+
						"Avg Surface Temp: -371 °F (-224 °C)";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "83%	hydrogen (H2)\n" +
							"15%	helium (He)\n" +
							"2.3%	methane (CH4)\n" +
							"0.009%\n" +
							"(0.007–0.015%)	hydrogen deuteride\n";
			}
		} else if(focus == "Neptune"){
			if (type == DescriptionType.DEFAULT){
				focusstr = 
					"Radius: 15,299 miles (24,622 km)\n"+
					"Mass: 102.4E24 kg (17.15 Earth mass)\n"+
					"Surface area: 2.941 billion sq miles (7.618 billion km²)\n"+
					"Distance from Sun: 2,798,000,000 miles (4,503,000,000 km)\n"+
					"Length of day: 16 hours 6 minutes.\n"+
					"Length of year: 165 years\n"+
					"Avg Surface Temp: -392 °F (-200 °C)";
			} else if (type == DescriptionType.COMPOSITION){
				focusstr = "80% hydrogen (H2)\n"+
							"19% helium (He)\n"+
							"1.5% methane (CH4)\n"+
							"0.019% hydrogen deuteride (HD)\n"+
							"0.00015% ethane (C2H6)\n";
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
