using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chara_Pointer : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		this.animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {

	}

	public void pointerEnter(){
		Debug.Log("pointer Enter");
		animator.SetBool("is_running", true);
	}

	public void pointerExit(){
		Debug.Log("pointer Exit");
		animator.SetBool("is_running", false);
	}
}
