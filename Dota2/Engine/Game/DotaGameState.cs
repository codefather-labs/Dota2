﻿using System.Collections.Generic;
using System.Linq;
using Dota2.Engine.Data;
using Dota2.Engine.Game.Data;
using Dota2.GC.Dota.Internal;

/*
    This file heavily based off of the nora project.
    See https://github.com/dschleck/nora/blob/master/lara/state/Client.cs
*/

namespace Dota2.Engine.Game
{
    /// <summary>
    /// Simulates the data stored in a DOTA 2 client.
    /// </summary>
    internal class DotaGameState
    {
        /// <summary>
        /// Connect details
        /// </summary>
        private DOTAConnectDetails _details;

        public int Baseline { get; set; }
        public uint ServerCount { get; set; }
        public float TickInterval { get; set; }
        public uint ClientTick { get; set; }
        public uint ServerTick { get; set; }
        public Dictionary<string, string> CVars { get; private set; }
        public List<StringTable> Strings { get; private set; }
        public Dictionary<string, int> StringsIndex { get; private set; }

        public List<EntityClass> Classes { get; private set; }
        public Dictionary<string, EntityClass> ClassesByName { get; private set; }
        public List<SendTable> SendTables { get; private set; }
        public List<FlatTable> FlatTables { get; private set; }
        public Dictionary<PropertyHandle, Property> Properties { get; private set; }
        public Dictionary<uint, Slot> Slots { get; private set; }

        public List<uint> Created { get; private set; }
        public List<uint> Deleted { get; private set; }

        /// <summary>
        /// Instantiates a new game state.
        /// </summary>
        /// <param name="details"></param>
        internal DotaGameState(DOTAConnectDetails details)
        {
            this._details = details;
            
            this.CVars = new Dictionary<string, string>();
            this.Strings = new List<StringTable>();
            this.StringsIndex = new Dictionary<string, int>();
            this.Classes = new List<EntityClass>();
            this.ClassesByName = new Dictionary<string, EntityClass>();
            this.SendTables = new List<SendTable>();
            this.FlatTables = new List<FlatTable>();
            this.Properties = new Dictionary<PropertyHandle, Property>();
            this.Slots = new Dictionary<uint, Slot>();

            this.Created = new List<uint>();
            this.Deleted = new List<uint>();

            Reset();
        }

        /// <summary>
        /// Reset the local data.
        /// </summary>
        public void Reset()
        {
            CVars.Clear();
            CVars.Add("tv_nochat", "0");
            CVars.Add("joy_autoaimdampen", "0");
            CVars.Add("name", _details.Name);
            CVars.Add("cl_interp_ratio", "2");
            CVars.Add("tv_listen_voice_indices", "0");
            CVars.Add("cl_predict", "0");
            CVars.Add("cl_updaterate", "30");
            CVars.Add("cl_showhelp", "1");
            CVars.Add("steamworks_sessionid_lifetime_client", "0");
            CVars.Add("cl_mouselook", "1");
            CVars.Add("steamworks_sessionid_client", "0");
            CVars.Add("dota_mute_cobroadcasters", "0");
            CVars.Add("voice_loopback", "0");
            CVars.Add("dota_player_initial_skill", "0");
            CVars.Add("cl_lagcompensation", "1");
            CVars.Add("closecaption", "0");
            CVars.Add("cl_language", "english");
            CVars.Add("english", "1");
            CVars.Add("cl_class", "default");
            CVars.Add("snd_voipvolume", "1");
            CVars.Add("snd_musicvolume", "1");
            CVars.Add("cl_cmdrate", "30");
            CVars.Add("net_maxroutable", "1200");
            CVars.Add("cl_team", "default");
            CVars.Add("rate", "80000");
            CVars.Add("cl_predictweapons", "1");
            CVars.Add("cl_interpolate", "1");
            CVars.Add("cl_interp", "0.05");
            CVars.Add("dota_camera_edgemove", "1");
            CVars.Add("snd_gamevolume", "1");
            CVars.Add("cl_spec_mode", "1");

            Classes.Clear();
            ClassesByName.Clear();
            SendTables.Clear();
            FlatTables.Clear();
            Properties.Clear();
            Slots.Clear();
            Strings.Clear();
            StringsIndex.Clear();

            Created.Clear();
            Deleted.Clear();
        }


        public CMsg_CVars ExposeCVars()
        {
            CMsg_CVars exposed = new CMsg_CVars();

            exposed.cvars.AddRange(CVars.Select(kv => {
                var var = new CMsg_CVars.CVar();
                var.name = kv.Key;
                var.value = kv.Value;
                return var;
            }));

            return exposed;
        }

        internal struct PropertyHandle
        {
            public uint Entity { get; set; }
            public string Table { get; set; }
            public string Name { get; set; }

            public override bool Equals(object o)
            {
                if (!(o is PropertyHandle))
                {
                    return false;
                }

                PropertyHandle handle = (PropertyHandle)o;
                return Entity == handle.Entity &&
                    Table.Equals(handle.Table) &&
                    Name.Equals(handle.Name);
            }

            public override int GetHashCode()
            {
                int result = (int)Entity;
                result = 31 * result + Table.GetHashCode();
                result = 31 * result + Name.GetHashCode();
                return result;
            }
        }

        internal class Slot
        {
            public Entity Entity { get; set; }
            public bool Live { get; set; }
            public Entity[] Baselines { get; set; }

            public Slot(Entity entity)
            {
                this.Entity = entity;
                this.Live = true;
                this.Baselines = new Entity[2];
            }
        }
    }
}