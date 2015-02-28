using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	public const float X_STEP = 4;//2;
	public const float Y_STEP = 4;//2;
	public const float Z_STEP = 4;//2;
	public const ulong NUM_X = 16;//32;
	public const ulong NUM_Y = 7;//14;
	public const ulong NUM_Z = 16;//32;
	public const float DOME_HEIGHT = 8;
	public GameObject Light;

	// default stuff
	private int time = 0;
	private const int NUM_FRAMES = 2;
	private const int NUM_STEPS = 10;
	public static bool defaultAnimation = false;
	private int currentGoal = 1;
	private int startStep = 0;
	private Color32[] rainbow = new Color32[7] { new Color32(0xFF, 0, 0, 0xFF),
												 new Color32(0xFF, 0xA5, 0, 0xFF),
												 new Color32(0xFF, 0xFF, 0, 0xFF),
												 new Color32(0, 0x80, 0, 0xFF),
												 new Color32(0, 0, 0xFF, 0xFF),
												 new Color32(0x4B, 0, 0x82, 0xFF),
												 new Color32(0xFF, 0, 0xFF, 0xFF) };

	public static GameObject[,,] LEDs = new GameObject[NUM_X,NUM_Z,NUM_Y];
	public static Color32[,,] LEDColors = new Color32[NUM_X,NUM_Z,NUM_Y];

	// Use this for initialization
	void Start () 
	{
		int i = 0;
		for (ulong x = 0; x < NUM_X; x++) 
		{
			for(ulong y = 0; y < NUM_Y; y++)
			{
				for(ulong z = 0; z < NUM_Z; z++)
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
						GameObject go = Instantiate(Light, new Vector3(x * X_STEP, (y + 1) * Y_STEP, z * Z_STEP), Quaternion.identity) as GameObject;
						LEDs[x,z,y] = go;
						LEDColors[x,z,y] = new Color32(0xff, 0, 0, 0xff);
					//	i++;
					//}
				}
			}
		}
		Debug.Log(string.Format("Number of lights: {0}", i));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(defaultAnimation)
		{
			time = (time++) % NUM_FRAMES;
			if (time == 0) 
			{
				int localGoal = currentGoal;
				int localStep = startStep;
				Color32 goal, start, current;

				for(ulong i = 0; i < NUM_X * NUM_Y * NUM_Z; i++)
				{
					goal = rainbow[localGoal];
					if(localGoal > 0)
					{
						start = rainbow[localGoal - 1];
					}
					else
					{
						start = rainbow[rainbow.Length - 1];
					}
					current = GetGradientColor(start, goal, NUM_STEPS, localStep);
					GetLED(i).renderer.material.color = current;
					if(localStep == NUM_STEPS)
					{
						localStep = 0;
						localGoal = (localGoal + 1) % rainbow.Length;
					}
					else
					{
						localStep++;
					}
				}

				if(startStep == NUM_STEPS)
				{
					startStep = 0;
					currentGoal = (currentGoal + 1) % rainbow.Length;
				}
				else 
				{
					startStep++;
				}
			}
		}
		else
		{
			for (ulong x = 0; x < NUM_X; x++) 
			{
				for(ulong y = 0; y < NUM_Y; y++)
				{
					for(ulong z = 0; z < NUM_Z; z++)
					{
						if(LEDs[x,z,y] != null)
						{
							LEDs[x,z,y].renderer.material.color = LEDColors[x,z,y];
						}
					}
				}
			}
		}
	}

	public static GameObject GetLED(ulong Index)
	{
		ulong x = Index / (NUM_Z * NUM_Y);
		ulong z = (Index % (NUM_Z * NUM_Y)) / NUM_Y;
		ulong y = (Index % (NUM_Z * NUM_Y)) % NUM_Y;

		return LEDs[x,z,y];
	}
	
	public static void SetLEDColor(ulong Index, Color32 Color)
	{
		ulong x = Index / (NUM_Z * NUM_Y);
		ulong z = (Index % (NUM_Z * NUM_Y)) / NUM_Y;
		ulong y = (Index % (NUM_Z * NUM_Y)) % NUM_Y;
		
		LEDColors[x,z,y] = Color;
	}
	
	public static Color32 GetLEDColor(ulong Index)
	{
		ulong x = Index / (NUM_Z * NUM_Y);
		ulong z = (Index % (NUM_Z * NUM_Y)) / NUM_Y;
		ulong y = (Index % (NUM_Z * NUM_Y)) % NUM_Y;
		
		return LEDColors[x,z,y];
	}

	Color32 GetGradientColor(Color32 start, Color32 goal, int numSteps, int currentStep)
	{
		byte redDelta, goalRed, startRed, redDiff, red;
		byte greenDelta, goalGreen, startGreen, greenDiff, green;
		byte blueDelta, goalBlue, startBlue, blueDiff, blue;
		
		goalRed = goal.r;
		goalGreen = goal.g;
		goalBlue = goal.b;
		
		startRed = start.r;
		startGreen = start.g;
		startBlue = start.b;
		
		redDelta = (byte)System.Math.Abs(goalRed - startRed);
		blueDelta = (byte)System.Math.Abs(goalBlue - startBlue);
		greenDelta = (byte)System.Math.Abs(goalGreen - startGreen);
		
		redDiff = (byte)((redDelta * currentStep) / numSteps);
		blueDiff = (byte)((blueDelta * currentStep) / numSteps);
		greenDiff = (byte)((greenDelta * currentStep) / numSteps);
		
		if(goalRed < startRed)
		{
			red = (byte)(startRed - redDiff);
		}
		else
		{
			red = (byte)(startRed + redDiff);
		}
		
		if(goalGreen < startGreen)
		{
			green = (byte)(startGreen - greenDiff);
		}
		else
		{
			green = (byte)(startGreen + greenDiff);
		}
		
		if(goalBlue < startBlue)
		{
			blue = (byte)(startBlue - blueDiff);
		}
		else
		{
			blue = (byte)(startBlue + blueDiff);
		}
		
		return new Color32(red, green, blue, 0xFF);
	}
}
