﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Master : MonoBehaviour {
	[Header("GameObjects")]
	public GameObject Camera;
	public GameObject Block_Prefab;
	public GameObject[] Blocks_Generated;
	[Header("Values")]
	public int Size = 100;
	public string Mode = "Random";
	public float Speed = .4f;
	public int[][] Sequence;
	[Header("Camera Perspective")]
	public bool View_2D = false;
	public bool View_3D = true;
	[Header("Text")]
	public TextAsset Input_Text;

	void Start () {
		//Setting to 3D View
		Camera.GetComponent<Transform>().position = new Vector3(Size*1.25f+3, Size*1.25f+3, Size*1.25f+3);
		Camera.GetComponent<Transform>().eulerAngles = new Vector3(30, 225, 0);
		//Generating Blocks
		Blocks_Generated = new GameObject[Size * Size];
		for (int i = 0; i < Size * Size; i++) {
			GameObject Clone = Instantiate(Block_Prefab, new Vector3(0 - ((Size-1) / 2f) + (Size - (i % Size) - 1), 0, 0 - ((Size-1) / 2f) + (int)Mathf.Floor(i / (float)Size)), Quaternion.identity);
			Clone.name = "Block #" + Linear_to_Matrix(i);
			Blocks_Generated[Linear_to_Matrix(i)] = Clone;
		}
		//StartCoroutine(Infinite());
	}

	void Update () {
		if (View_3D == true && View_2D == true) {
			if (Camera.GetComponent<Transform>().position.x == Size*1.25f+3) { //3D Initial
				Camera.GetComponent<Transform>().position = new Vector3(0, Size*1.25f+3, 0);
				Camera.GetComponent<Transform>().eulerAngles = new Vector3(90, 180, 0);
				View_3D = false;
			}else { //2D Initial
				Camera.GetComponent<Transform>().position = new Vector3(Size*1.25f+3, Size*1.25f+3, Size*1.25f+3);
				Camera.GetComponent<Transform>().eulerAngles = new Vector3(30, 225, 0);
				View_2D = false;
			}
		}
	}

	IEnumerator TextToASCII (string Input) {
		foreach(char x in Input) {
			ASCII((int)x-32,0); //start the ascii table with the 32nd char
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator Cycle () {
		for (int i = 0; i < 94; i++) {
			ASCII(i,0);
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator Infinite () { //TODO: Make work!
		for (int i = 0; i < 94; i++) {
			for (int a = -22; a < 100; a++) {
				ASCII((int)'F'-32, a);
				yield return new WaitForSeconds(.2f);
			}
		}
	}

	int Linear_to_Matrix (int Input) { //Rearranges number
		int x = Input % Size;
		int y = (int)Mathf.Floor(Input / Size);
		int temp_x = x; //Optimize?
		if (x % 2 == 0) {
			x = Size-y-1;
		}else {
			x = y;
		}
		y = Size-temp_x-1;
		x = (int)Mathf.Floor(x / Size) + (Size - (x % Size) - 1); //Optimize?
		return y*Size+x;
	}

	int Matrix_to_Linear (int Input) { //Rearranges number
		int x = Input % Size;
		int y = (int)Mathf.Floor(Input / Size);
		int temp_y = y; //Optimize?
		if (y % 2 == 0) {
			y = Size-x-1;
		}else {
			y = x;
		}
		x = temp_y;
		x = (int)Mathf.Floor(x / Size) + (Size - (x % Size) - 1);
		return y*Size+x;
	}

	void ASCII (int Number, int offset) { //TODO: Get rid of wrap effect
		foreach (GameObject x in Blocks_Generated) { //Reset function
			x.GetComponent<Renderer>().material.color = new Color(255,255,255,255);
		}
		StreamReader Input = new StreamReader(new MemoryStream(Input_Text.bytes));
		string Line = null;
		for (int i = 0; i < Number; i++) {
			Line = Input.ReadLine();
		}
		if (Line != null) {
			string[] Coordinates = new string[Line.Length/2]; //Optimize
			for (int i = 0, a = 0; i < Line.Length; i++) {
				if (i == 0) {
					Coordinates[a] += Line[0];
					if (Line[1] != ',') {
						Coordinates[a] += Line[1];
					}
					Coordinates[a] = Linear_to_Matrix(Matrix_to_Linear(int.Parse(Coordinates[a]))-offset).ToString(); //temp
					a++;
				}
				if (Line[i] == ',') {
					Coordinates[a] += Line[i+1];
					if (Line[i+2] != ',') {
						Coordinates[a] += Line[i+2];
					}
					Coordinates[a] = Linear_to_Matrix(Matrix_to_Linear(int.Parse(Coordinates[a]))-offset).ToString(); //temp
					a++;
				}
			}
			for (int i = 0; i < Coordinates.Length; i++) {
				if (Coordinates[i] != null) {
					Blocks_Generated[int.Parse(Coordinates[i])].GetComponent<Renderer>().material.color = new Color(255,0,0,255);
				}
			}
		}
		Input.Close();
	}
}
