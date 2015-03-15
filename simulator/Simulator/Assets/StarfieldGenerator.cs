using UnityEngine;
using System.Collections;
using System;

public class StarfieldGenerator : MonoBehaviour {
	public const float DEFAULT_X_STEP = 4;//2;
	public const float DEFAULT_Y_STEP = 2;
	public const float DEFAULT_Z_STEP = 4;//2;
	public const ulong DEFAULT_NUM_X = 16;//32;
	public const ulong DEFAULT_NUM_Y = 15;//14;
	public const ulong DEFAULT_NUM_Z = 16;//32;
	public const float DEFAULT_DOME_HEIGHT = 8;
	public GameObject Light;
	public bool BurningMan = true;
	public bool Critical = false;
	public bool Home = false;
	public bool Custom = false;

	// 4' in meters = 1.2192
	// 2' in meters = .6096
	// 1' in meters = .3048
	public static float XStep = 4;
	public static float YStep = 2;
	public static float ZStep = 4;
	public static ulong NumX = 16;
	public static ulong NumY = 15;
	public static ulong NumZ = 16;
	public static float LowestHeight = 1;
	
	string numXText, numYText, numZText, xStepText, yStepText, zStepText;

	public static GameObject[,,] LEDs = new GameObject[NumX,NumZ,NumY];
	public static Color32[,,] LEDColors = new Color32[NumX,NumZ,NumY];

	void OnGUI()
	{
		if(GUI.Toggle(new Rect(10, 0, 100, 20), BurningMan, "Burning Man"))
		{
			if(!BurningMan)
			{
				Home = false;
				Critical = false;
				Custom = false;
			}
			BurningMan = true;
		}

		if(GUI.Toggle(new Rect(10, 20, 100, 20), Critical, "Critical NW"))
		{
			if(!Critical)
			{
				Home = false;
				BurningMan = false;
				Custom = false;
			}
			Critical = true;
		}

		if(GUI.Toggle(new Rect(10,40,100, 20), Home, "Home"))
		{
			if(!Home)
			{
				Critical = false;
				BurningMan = false;
				Custom = false;
			}
			Home = true;
		}
		
		if(GUI.Toggle(new Rect(10,60,100, 20), Custom, "Custom"))
		{
			if(!Custom)
			{
				Critical = false;
				BurningMan = false;
				Home = false;
			}
			Custom = true;
		}

		if(Custom)
		{

			GUI.Label(new Rect(10, 80, 50, 20), "NumX");
			numXText = GUI.TextField(new Rect(60, 80, 100, 20), numXText);
			
			GUI.Label(new Rect(10, 100, 50, 20), "NumY");
			numYText = GUI.TextField(new Rect(60, 100, 100, 20), numYText);
			
			GUI.Label(new Rect(10, 120, 50, 20), "NumZ");
			numZText = GUI.TextField(new Rect(60, 120, 100, 20), numZText);
			
			GUI.Label(new Rect(10, 140, 50, 20), "X Step");
			xStepText = GUI.TextField(new Rect(60, 140, 100, 20), xStepText);
			
			GUI.Label(new Rect(10, 160, 50, 20), "Y Step");
			yStepText = GUI.TextField(new Rect(60, 160, 100, 20), yStepText);
			
			GUI.Label(new Rect(10, 180, 50, 20), "Z Step");
			zStepText = GUI.TextField(new Rect(60, 180, 100, 20), zStepText);
			
			if(GUI.Button(new Rect(10,200,100,20), "Regenerate"))
			{
				try
				{
					float tempXStep, tempYStep, tempZStep;
					ulong tempNumX, tempNumY, tempNumZ;
					tempXStep = float.Parse(xStepText);
					tempYStep = float.Parse(yStepText);
					tempZStep = float.Parse(zStepText);
					tempNumX = ulong.Parse(numXText);
					tempNumY = ulong.Parse(numYText);
					tempNumZ = ulong.Parse(numZText);

					XStep = tempXStep;
					YStep = tempYStep;
					ZStep = tempZStep;
					NumX = tempNumX;
					NumY = tempNumY;
					NumZ = tempNumZ;

					Regenerate();
				}
				catch(Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
		else
		{
			if(GUI.Button(new Rect(10,80,100,20), "Regenerate"))
			{
				if(BurningMan)
				{
					XStep = 4;
					YStep = 2;
					ZStep = 4;
					NumX = 16;
					NumY = 15;
					NumZ = 16;

					Regenerate();
				}
				else if(Critical)
				{
					XStep = 2;
					YStep = 2;
					ZStep = 2;
					NumX = 11;
					NumY = 10;
					NumZ = 11;

					Regenerate();
				}
				else if(Home)
				{
					XStep = 2;
					YStep = 2;
					ZStep = 2;
					NumX = 7;
					NumY = 4;
					NumZ = 5;

					Regenerate();
				}
			}
		}
	}

	void Regenerate()
	{
		int i = 0;
		for(int x = 0; x < LEDs.GetLength(0); x++)
		{
			for(int y = 0; y < LEDs.GetLength(2); y++)
			{
				for(int z = 0; z < LEDs.GetLength(1); z++)
				{
					Destroy(LEDs[x,z,y]);
				}
			}
		}

		LEDs = new GameObject[NumX,NumZ,NumY];
		LEDColors = new Color32[NumX,NumZ,NumY];

		for (ulong x = 0; x < NumX; x++) 
		{
			for(ulong y = 0; y < NumY; y++)
			{
				for(ulong z = 0; z < NumZ; z++)
				{
					/*if((y > 3) || 
					   (x < 3) || (x >= (NUM_X - 3)) ||
					   (z < 3) || (z >= (NUM_Z - 3)) ||
					   ((x == 4) && (y >= 2)) ||
					   ((x == 5) && (y >= 3)) ||
					   ((x == NUM_X - 4) && (y >= 2)) ||
					   ((x == NUM_X - 5) && (y >= 3)) ||
					   ((z == 4) && (y >= 2)) ||
					   ((z == 5) && (y >= 3)) ||
					   ((z == NUM_Z - 4) && (y >= 2)) ||
					   ((z == NUM_Z - 5) && (y >= 3)) ||
					   (x % 2 == 0 && z % 2 == 0))
					{*/
					GameObject go = Instantiate(Light, new Vector3(x * XStep, (y + LowestHeight) * YStep, z * ZStep), Quaternion.identity) as GameObject;
					LEDs[x,z,y] = go;
					LEDColors[x,z,y] = new Color32(0xff, 0, 0, 0xff);
					i++;
					//}
				}
			}
		}

		numXText = NumX.ToString();
		numYText = NumY.ToString();
		numZText = NumZ.ToString();
		xStepText = XStep.ToString();
		yStepText = YStep.ToString();
		zStepText = ZStep.ToString();

		Debug.Log(string.Format("Number of lights: {0}", i));
	}

	// Use this for initialization
	void Start () 
	{
		Regenerate();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (ulong x = 0; x < NumX; x++) 
		{
			for(ulong y = 0; y < NumY; y++)
			{
				for(ulong z = 0; z < NumZ; z++)
				{
					if(LEDs[x,z,y] != null)
					{
						LEDs[x,z,y].renderer.material.color = LEDColors[x,z,y];
					}
				}
			}
		}
	}

	public static GameObject GetLED(ulong Index)
	{
		ulong x = Index / (NumZ * NumY);
		ulong z = (Index % (NumZ * NumY)) / NumY;
		ulong y = (NumY - 1) - (Index % (NumZ * NumY)) % NumY;

		return LEDs[x,z,y];
	}
	
	public static void SetLEDColor(ulong Index, Color32 Color)
	{
		ulong x = Index / (NumZ * NumY);
		ulong z = (Index % (NumZ * NumY)) / NumY;
		ulong y = (NumY - 1) - (Index % (NumZ * NumY)) % NumY;
		
		LEDColors[x,z,y] = Color;
	}
	
	public static Color32 GetLEDColor(ulong Index)
	{
		ulong x = Index / (NumZ * NumY);
		ulong z = (Index % (NumZ * NumY)) / NumY;
		ulong y = (NumY - 1) - (Index % (NumZ * NumY)) % NumY;
		
		return LEDColors[x,z,y];
	}
}
