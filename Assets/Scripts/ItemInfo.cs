﻿using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class ItemInfo : MonoBehaviourPunCallbacks,IPunObservable
{
     [SerializeField] public string weaponname;
     private PhotonView PV;
     public int setazero = 0;
     private void Start()
     {
          Random rng = new Random();
          weaponname = WichItem((rng.Next(9)+setazero)%10);
          PV = GetComponent<PhotonView>();
          if (PV.IsMine)
          {
               Random rng = new Random();
               weaponname = WichItem((rng.Next(10)+setazero)%11);
          }
     }
     private string WichItem(int rng)
     {
          switch (rng)
          {
               case 0:
                    return "rafale";
               case 1:
                    return "masse";
               case 2:
                    return "poisondart";
               case 3:
                    return "laserbeam";
               case 4:
                    return "aoeattack";
               case 5:
                    return "aoeheal";
               case 6:
                    return "moreshoot";
               case 7:
                    return "mine";
               case 8:
                    return "canon";
               case 9:
                    return "instantheal";
               case 10:
                    return "shield";
               case 11:
                    return "seisme";
               default:
                    return "laserbeam";
          }
     }

     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {
          if (stream.IsWriting)
          {
               stream.SendNext(weaponname);
          }
          else if(stream.IsReading);

          {
               weaponname = (string) stream.ReceiveNext();
          }
     }
}
