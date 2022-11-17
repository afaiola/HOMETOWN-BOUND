/* Copyright (c) Dr. Stephen Ware, the University of Kentucky, and the University of New Orleans. All rights reserved.
* Lead Developer: Alireza Shirvani
* Additional Development: Edward T. ("ET") Garcia
* Licensed under the Non-Profit Open Software License version 3.0. See LICENSE file in the project root for full license information. */using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cakeslice
{
    public class MaterialSwitcher : MonoBehaviour
    {
        public Material target;
        public int index;

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                Material[] materials = GetComponent<Renderer>().materials;
                materials[index] = target;
                GetComponent<Renderer>().materials = materials;
            }
        }
    }
}