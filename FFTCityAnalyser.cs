using UnityEngine;
using System.Collections;

/*
 *  FFTCityAnalizer class to make city dance with music:
 *	By: Maria Alejandra Montenegro
 *		 2013
 * */
 
public class FFTCityAnalizer : MonoBehaviour {
	
	// main class variables
	public float []freqData = new float[8192];
	public float [] band;
	public CityController cityControl;
	public DragonController dragonyControl;
	public popController shapeController;
	bool isPop = false;
	bool popDead = false;
	
	bool first = true;
	float myTime;
	
    float spacing;
    float width;
	
	public float frequency;
	public float max;
	
	// Use this for initialization
	void Start () {
		
		//Filling in the frequency Data
		int n = freqData.Length;	
		int k = 0;
		myTime = Time.time;
		for(int i=0;i<freqData.Length;i++){
			n /=2;
			//break n empty   
			if(n == 0)
				break;
			k++;
		}
		band  = new float[k+1];
		//g = new GameObject[k+1];
		
	    for (int i=1;i<band.Length;i++){
			band[i] = 0;	
		}
	
		//Creating function to run/ update our bands
		InvokeRepeating("check", 0, 1.0f/25.0f); // update at 25 fps	
	}
	
	// Update is called once per frame
	void Update () {
		if(!cityControl.isAlive && ! popDead){
			popDead = true;
			dragonyControl.isDragon  = true;
			shapeController.destroyAll();
		}
		
	}

	
	// Main FFT Function: check FFT info and update
	void check(){
		
		AudioListener.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
		int k = 0;
		int crossOver = 2;
		
		float [] bandInfo = new float[14];
		int bb=0;
		for(int i = 0;i < freqData.Length;i++){
			float d = freqData[i];
			float b = band[k];
			band[k] = (d>b)? d:b;   // find the max as the peak value in that frequency band.
			
			if (i>crossOver-3){
				k++;
				bb++;
				crossOver *=2; // frequency crossover point for each band.
				float posY = band[k]*32;
				bandInfo[bb] = posY;
				
				//Check if time to make city dance and emit particles :)
				if(cityControl.isAlive){
					//print (posY);
					//if(k == 3 || k==4|| k ==5){
						if(posY > 3.34f && !isPop && !first){ //7.6
							//print (posY);
							StartCoroutine(popShapes());
						}
						if((Time.time - myTime) > 29.5f && first && !isPop){
							first = false;
							StartCoroutine(popShapes());
							//StartCoroutine(popShapes());
					//	}
					}
				}
				band[k] = 0;
			}
		}
		if(cityControl.isAlive){
			cityControl.updateBuildings(bandInfo);
		}
		if(dragonyControl.isDragon)
			dragonyControl.updateDragon(bandInfo);
	}

	//Function changes color to city
	void changeColor(Color color, GameObject gm){
		foreach(Transform child in gm.transform){
			child.renderer.material.color = Color.Lerp(child.renderer.material.color,color,1f);
			
		}
	}
	//Function that calls particle emitter for dancing city
	IEnumerator popShapes(){
	
		StartCoroutine(shapeController.createShapes());	
		yield return new  WaitForSeconds(0.2f);
		StartCoroutine(shapeController.createShapes());	
		isPop =false;
	}
}
