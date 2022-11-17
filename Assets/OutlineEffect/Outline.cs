/* Copyright (c) Dr. Stephen Ware, the University of Kentucky, and the University of New Orleans. All rights reserved.
* Lead Developer: Alireza Shirvani
* Additional Development: Edward T. ("ET") Garcia
* Licensed under the Non-Profit Open Software License version 3.0. See LICENSE file in the project root for full license information. *//*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace cakeslice
{
    [ExecuteInEditMode]
    public class Outline : MonoBehaviour
    {
        public Renderer Renderer { get; private set; }

        public int color;
        public bool eraseRenderer;

        [HideInInspector]
        public int originalLayer;
        [HideInInspector]
        public Material[] originalMaterials;

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
        }

        void OnEnable()
        {
            IEnumerable<OutlineEffect> effects = Camera.allCameras.AsEnumerable()
                .Select(c => c.GetComponent<OutlineEffect>())
                .Where(e => e != null);

            foreach (var renderer in GetComponentsInChildren<Renderer>())
                if (!renderer.GetComponent<Outline>() && !renderer.GetComponent<IgnoreOutline>())
                    renderer.gameObject.AddComponent<Outline>();

            foreach (OutlineEffect effect in effects)
            {
                effect.AddOutline(this);
            }
        }

        void OnDisable()
        {
            IEnumerable<OutlineEffect> effects = Camera.allCameras.AsEnumerable()
                .Select(c => c.GetComponent<OutlineEffect>())
                .Where(e => e != null);

            foreach (var outline in GetComponentsInChildren<Outline>())
                if (outline != this)
                    outline.enabled = false;

            foreach (OutlineEffect effect in effects)
            {
                effect.RemoveOutline(this);
            }
        }
    }
}