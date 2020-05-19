﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = System.Random;

public class matrixe : MonoBehaviour
{
    // 1 -> Top || 2 -> Bot || 3 -> Left || 4 -> Right || 5 -> Spawn || 6 -> Boss
    // 7-> forgeron || 8-> shop || 9-> instructeur || 10 -> cook || 11-> item
    [SerializeField] public (bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool)[,] matrix;
     public int size;
    [SerializeField] private GameObject neo;
    //[SerializeField] private GameObject boss;
    private Random r = new Random();
    
    private PhotonView PV;
    public bool shouldGen = true;
    
    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("is generating");
            PV = gameObject.GetComponent<PhotonView>();
            if (size % 2 == 0) size += 1;
            matrix = new (bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool)[size,size]; 
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = (true, true, true, true, false, false, false, false, false, false, false);
                }
            }
        
            generatedungeon();
            shouldGen = false;
            
            int cnt = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!(matrix[i, j].Item1 && matrix[i, j].Item2 && matrix[i, j].Item3 && matrix[i, j].Item4))
                    {
                        cnt++;
                        PV.RPC("Generate", RpcTarget.AllBuffered, i, j, cnt);
                    }
                }
            }
        }
        else
            Debug.Log("not gonna generate");
    }

    public void generatedungeon()
    {
        int maxroom = (size * size) /3;
        int compteur = 5;
        bool boule = true;
        
//        Debug.Log("GenerateDungeon : Setting things up for start room");
        matrix[size / 2, size / 2].Item1 = false;
        matrix[size / 2, size / 2].Item2 = false;
        matrix[size / 2, size / 2].Item3 = false;
        matrix[size / 2, size / 2].Item4 = false;
        matrix[size / 2, size / 2].Item5 = true;
        matrix[size / 2, size / 2].Item6 = false;


        while (compteur < maxroom && boule)
        {
            (int x, int y)= recdungeon();
            if (x >= 0)
            {
                int a = generateroom(x, y, maxroom - compteur);
                compteur += a;
            }
            else boule = false;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                checkdoors(i,j);
            }
        }

        AddTheBossRoom();
        SpecialRooms();
    }
    public (int,int) recdungeon()
    {
//        Debug.Log("Recdungeon : Called");
        List<(int,int)> a = new List<(int, int)>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (IsAccesible(i, j)) 
                    a.Add((i,j));
            }
        }
        int b = 0;
        int c = a.Count-1;
        if (c >= 0)
            b += r.Next(c);
        if (c == -1)
            return (-1, -1);
        (int x, int y) = a[b];
        return (x,y);
    }
    
    
    public bool isvalid(int i ,int j)
    {
        return (i >= 0 && j >= 0 && i < size && j < size);
    }
    
    public bool ishere(int i, int j)
    {
        if (isvalid(i, j))
        {
            return !(matrix[i, j].Item1 && matrix[i, j].Item2 && matrix[i, j].Item3 && matrix[i, j].Item4);
        }
        return false;
    }
    
    public bool IsAccesible(int i, int j)
    {
        bool b = false;
        if (!ishere(i,j))
        {
            if (isvalid(i,j+1) && !matrix[i,j+1].Item2)
                b = true;
            else if (isvalid(i,j-1) && !matrix[i,j-1].Item1)
                b = true;
            else if (isvalid(i+1,j) && !matrix[i+1,j].Item3)
                b = true;
            else if (isvalid(i-1,j) && !matrix[i-1,j].Item4)
                b = true;
        }

        return b;
    }
    
    
    public int generateroom(int i, int j,int diff)
    {
//        Debug.Log("GenerateRoom : Called");
        int size = matrix.GetLength(0);
        int compteur = 0;
        
        
        checkdoors(i,j);
        int d = possibledirections(size, i, j);


        if (d >= 3 && diff >= 3) 
        { 
            int a = r.Next(4);
            if (a == 3)
                compteur += randomdoor3(i, j);
            if (a == 2) 
                compteur += randomdoor2(i, j);
            if (a == 1 )  
                compteur += randomdoor1(i, j);
        }
        else if (d >= 2 && diff >= 2) 
        { 
            int a = r.Next(3); 
            if (a == 2) 
                compteur += randomdoor2(i, j);
            if (a == 1|| a == 0) 
                compteur += randomdoor1(i, j);
        }
        else if (d >= 1 && diff >= 1)
        {
            int a = r.Next(2);
            if (a == 1)
                compteur += randomdoor1(i, j);
        }

        return compteur;
    }
    
    public int possibledirections(int size, int i, int j)
    {
        int d = 0;
        if (!ishere(i + 1, j) && isvalid( i + 1, j))
            d++;
        if (!ishere( i - 1, j) && isvalid( i - 1, j))
            d++;
        if (!ishere( i, j + 1) && isvalid( i, j + 1))
            d++;
        if (!ishere( i, j - 1) && isvalid( i, j - 1))
            d++;
        return d;
    }
    
    public void checkdoors(int i, int j)
    {
        if (isvalid(i + 1, j) && !matrix[i + 1, j].Item3)
            matrix[i, j].Item4 = false;

        if (isvalid(i - 1, j) && !matrix[i - 1, j].Item4)
            matrix[i, j].Item3 = false;
        
        if (isvalid(i,j+1) && !matrix[i,j+1].Item2)
            matrix[i, j].Item1 = false;
        
        if (isvalid(i,j-1) && !matrix[i,j-1].Item1)
            matrix[i, j].Item2 = false;
        
        if (!isvalid(i+1,j) && !matrix[i,j].Item4)
            matrix[i, j].Item4 = true;
        
        if (!isvalid(i-1,j) && !matrix[i,j].Item3)
            matrix[i, j].Item3 = true;
        
        if (!isvalid(i,j+1) && !matrix[i,j].Item1)
            matrix[i, j].Item1 = true;
        
        if (!isvalid(i,j-1) && !matrix[i,j].Item2)
            matrix[i, j].Item2 = true;
    }


    public int randomdoor3(int i, int j)
    {
        int a = 0;
        a += randomdoor1(i, j);
        a += randomdoor2(i, j);
        return a;
    }
    
    
    //fonction qui creuse 2 portes 
    public int randomdoor2(int i, int j)
    {
        int a = 0;
        a += randomdoor1(i, j);
        a += randomdoor1(i, j);
        return a;
    }
    
    
    //fonction qui creuse 1 porte aléatoirement
    public int randomdoor1(int i, int j)
    {
        int size = matrix.GetLength(0);
        bool added = false;
        
        while (!added && possibledirections(size,i,j)!=0)
        {
            int a = r.Next(4);
                    
            if (a == 0 && isvalid(i, j + 1) && matrix[i, j].Item4)
            {
                added = true;
                matrix[i, j].Item4 = false;
            }
            if (a == 1 && isvalid( i, j - 1) && matrix[i, j].Item3)
            {
                added = true;
                matrix[i, j].Item3 = false;
            }
            if (a == 2 && isvalid(i+1, j ) && matrix[i, j].Item2)
            {
                added = true;
                matrix[i, j].Item2 = false;
            }
            if (a == 3 && isvalid( i-1, j) && matrix[i, j].Item1)
            { 
                added = true;
                matrix[i, j].Item1 = false;
            }
        }

        if (added)
        {
            return 1;
        }
        return 0;
    }

    public void AddTheBossRoom()
    {
        int rand = r.Next(4);
        if (rand == 0)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (ishere( i, j) && IsBossCandidaite(i,j))
                    {
                        matrix[i, j].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
        if (rand == 1)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (ishere( j, i) && IsBossCandidaite(j,i))
                    {
                        matrix[j, i].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
        if (rand == 2)
        {
            for (int i = size; i > 0; i--)
            {
                for (int j = size; j > 0; j--)
                {
                    if (ishere( i, j) && IsBossCandidaite(i,j))
                    {
                        matrix[i, j].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
        if (rand == 3)
        {
            for (int i = size; i > 0; i--)
            {
                for (int j = size; j > 0; j--)
                {
                    if (ishere( j, i) && IsBossCandidaite(j,i))
                    {
                        matrix[j, i].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
    }

    public bool IsBossCandidaite( int i, int j)
    {
        bool bool1 = !matrix[i, j].Item1;
        bool bool2 = !matrix[i, j].Item2;
        bool bool3 = !matrix[i, j].Item3;
        bool bool4 = !matrix[i, j].Item4;

        bool bool5 = bool1 && !bool2 && !bool3 && !bool4;
        bool bool6 = !bool1 && bool2 && !bool3 && !bool4;
        bool bool7 = !bool1 && !bool2 && bool3 && !bool4;
        bool bool8 = !bool1 && !bool2 && !bool3 && bool4;

        bool bool9 = isvalid( i, j + 1) && matrix[i, j + 1].Item5;
        bool bool10 = isvalid(i, j - 1) && matrix[i, j - 1].Item5;
        bool bool11 = isvalid(i + 1, j) && matrix[i + 1, j].Item5;
        bool bool12 = isvalid(i - 1, j) && matrix[i - 1, j].Item5;

        bool bool13 = bool9 || bool10 || bool11 || bool12;

        return (bool5 || bool6 || bool7 || bool8) && !bool13;
    }


    public void generateforest(GameObject gobj, int i, int j)
    {
        int tw = r.Next(0,2);
        int bw = r.Next(0,2);
        int lw = r.Next(0,2);
        int rw = r.Next(0,2);
        int gr = r.Next(0,2);
        
        
        if (tw==0)
        {
            gobj.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            
            if (matrix[i,j].Item1)
            {
                gobj.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                gobj.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            gobj.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
            
            if (matrix[i,j].Item1)
            {
                gobj.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
            }
        }
        if (bw==0)
        {
            gobj.transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
            
            if (matrix[i,j].Item2)
            {
                gobj.transform.GetChild(3).GetChild(2).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            gobj.transform.GetChild(2).GetChild(3).gameObject.SetActive(true);
            
            if (matrix[i,j].Item2)
            {
                gobj.transform.GetChild(3).GetChild(3).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(2).gameObject.SetActive(true);
            }
        }
        if (lw==0)
        {
            gobj.transform.GetChild(2).GetChild(4).gameObject.SetActive(true);
            
            if (matrix[i,j].Item3)
            {
                gobj.transform.GetChild(3).GetChild(4).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(3).gameObject.SetActive(true);
            }
        }
        else
        {
            gobj.transform.GetChild(2).GetChild(5).gameObject.SetActive(true);
            
            if (matrix[i,j].Item3)
            {
                gobj.transform.GetChild(3).GetChild(5).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(4).gameObject.SetActive(true);
            }
        }
        if (rw==0)
        {
            gobj.transform.GetChild(2).GetChild(6).gameObject.SetActive(true);
            
            if (matrix[i,j].Item4)
            {
                gobj.transform.GetChild(3).GetChild(6).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(5).gameObject.SetActive(true);
            }
        }
        else
        {
            gobj.transform.GetChild(2).GetChild(7).gameObject.SetActive(true);
            
            if (matrix[i,j].Item4)
            {
                gobj.transform.GetChild(3).GetChild(7).gameObject.SetActive(true);

            }
            else
            {
                gobj.transform.GetChild(4).GetChild(6).gameObject.SetActive(true);
            }
        }

        gobj.transform.GetChild(1).gameObject.SetActive(gr==0);
    }

    public void SpecialRooms()
    {
        bool item = true;
        while (item)
        {
            int a = r.Next(size);
            int b = r.Next(size);
            if (IsBossCandidaite(a, b))
            {
                matrix[a, b].Item11 = true;
                item = false;
            }
        }
        if (size<=9)
        {
            
            bool market = true;
            while (market)
            {
                int a = r.Next(size);
                int b = r.Next(size);
                if (ishere(a, b) && !matrix[a, b].Item11 && !matrix[a,b].Item5 && !matrix[a,b].Item6) 
                {
                    matrix[a, b].Item7 = true;
                    matrix[a, b].Item8 = true;
                    matrix[a, b].Item9 = true;
                    matrix[a, b].Item10 = true;
                    market = false;
                }
            }
        }
        else
        {
            bool forg = true;
            while (forg)
            {
                int a = r.Next(size);
                int b = r.Next(size);
                if (ishere(a, b) && !matrix[a, b].Item11 && !matrix[a,b].Item5 && !matrix[a,b].Item6) 
                {
                    matrix[a, b].Item7 = true;
                    forg = false;
                }
            }
            bool shop = true;
            while (shop)
            {
                int a = r.Next(size);
                int b = r.Next(size);
                if (ishere(a, b) && !matrix[a, b].Item11 && !matrix[a,b].Item5 && !matrix[a,b].Item6) 
                {
                    matrix[a, b].Item8 = true;
                    shop = false;
                }
            }
            bool ins = true;
            while (ins)
            {
                int a = r.Next(size);
                int b = r.Next(size);
                if (ishere(a, b) && !matrix[a, b].Item11 && !matrix[a,b].Item5 && !matrix[a,b].Item6) 
                {
                    matrix[a, b].Item9 = true;
                    ins = false;
                }
            }
            bool cook = true;
            while (cook)
            {
                int a = r.Next(size);
                int b = r.Next(size);
                if (ishere(a, b) && !matrix[a, b].Item11 && !matrix[a,b].Item5 && !matrix[a,b].Item6) 
                {
                    matrix[a, b].Item10 = true;
                    cook = false;
                }
            }
        }
    }
    
    [PunRPC]
    private void Generate(int i, int j, int counter)
    {
        //Instantiating
        GameObject oo = Instantiate(neo, new Vector2(i*19, j*12), Quaternion.identity);

        //If is Spawn
        if (matrix[i, j].Item5)
        {
            //Put GameSetup here
            GameObject.Find("GameSetup").transform.position = new Vector3(i*19, j*12);
        }

        oo.name = $"Room_{counter}";
        
        //Generating Walls/etc...
        generateforest(oo,i,j);
        
        //Setting variables
        oo.GetComponent<cleanscript>().spawn = matrix[i, j].Item5;
        oo.GetComponent<cleanscript>().boss = matrix[i, j].Item6;
        oo.GetComponent<cleanscript>().forge = matrix[i, j].Item7;
        oo.GetComponent<cleanscript>().shop = matrix[i, j].Item8;
        oo.GetComponent<cleanscript>().instructor = matrix[i, j].Item9;
        oo.GetComponent<cleanscript>().cook = matrix[i, j].Item10;
        oo.GetComponent<cleanscript>().item = matrix[i, j].Item11;
    }
}


