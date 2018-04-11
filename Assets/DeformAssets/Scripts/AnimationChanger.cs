using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class AnimationChanger : MonoBehaviour {

    Animator animator;

    public RuntimeAnimatorController[] controllers;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void ChangeAnimation(Dropdown dropdown)
    {
        if (animator == null) return;

        int id = dropdown.value;
        animator.runtimeAnimatorController = controllers[id];
    }
}