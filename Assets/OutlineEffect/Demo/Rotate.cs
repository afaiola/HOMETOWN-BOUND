/* Copyright (c) Dr. Stephen Ware, the University of Kentucky, and the University of New Orleans. All rights reserved.
* Lead Developer: Alireza Shirvani
* Additional Development: Edward T. ("ET") Garcia
* Licensed under the Non-Profit Open Software License version 3.0. See LICENSE file in the project root for full license information. */using UnityEngine;
using System.Collections;
using cakeslice;

namespace cakeslice
{
    public class Rotate : MonoBehaviour
    {
        public float speed = 20f;
        float timer;
        const float time = 1;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * speed);

            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = time;
                //GetComponent<Outline>().enabled = !GetComponent<Outline>().enabled;
            }
        }
    }
}