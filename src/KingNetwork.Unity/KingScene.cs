using System;
using System.Collections.Generic;
using UnityEngine;

namespace KingNetwork.Unity
{
    /// <summary>
    /// King Scene
    /// This class is responsible for synchronizing the scene.
    /// </summary>
    [
        AddComponentMenu("KingNetwork/King Scene")
    ]
    public class KingScene : MonoBehaviour
    {
        /// <summary>Syncs by identifier</summary>
        private IDictionary<short, KingSync> _syncsById;

        /// <summary>Syncs</summary>
        private IList<KingSync> _syncs;


        /// <summary>Sync identifier</summary>
        private short SyncId;

        /// <summary>Sync time</summary>
        private float SyncTime;

        /// <summary>Has spawn</summary>
        private bool HasSpawn;


        /// <summary>On unpawn</summary>
        public Action<KingSync> OnUnpawning;

        /// <summary>On spawning</summary>
        public Action<KingSync> OnSpawning;


        /// <summary>Scope radius</summary>
        public int ScopeRadius;

        /// <summary>Sync rate</summary>
        public int SyncRate;


        /// <summary>Is empty</summary>
        public bool IsEmpty
        {
            get { return (_syncs.Count == 0); }
        }

        /// <summary>Count</summary>
        public int Count
        {
            get { return _syncs.Count; }
        }


        /// <summary>Clear</summary>
        public void Clear()
        {
            // Unspawn all sync identifiers
            foreach (KingSync sync in _syncs)
                Unspawn(sync.SyncId);
        }

        /// <summary>Clear others</summary>
        /// <param name="syncId">Sync identifier</param>
        public void ClearOthers(short syncId)
        {
            // Unspawn others sync identifier
            foreach (KingSync sync in _syncs)
                if (sync.SyncId != syncId) Unspawn(sync.SyncId);
        }


        /// <summary>Find network sync</summary>
        /// <param name="syncId">Sync identifier</param>
        public KingSync Find(short syncId)
        {
            if (_syncsById.ContainsKey(syncId)) return null;
            return _syncsById[syncId];
        }


        /// <summary>Spawn</summary>
        /// <param name="prefab">Prefab</param>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        public KingSync Spawn(string prefab, Vector3 position, Quaternion rotation)
        {
            // Has spawn
            HasSpawn = true;

            // Spawn
            return Spawn(prefab, position, rotation, NextsyncId());
        }

        /// <summary>Spawn</summary>
        /// <param name="prefab">Prefab</param>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="syncId">sync identifier</param>
        public KingSync Spawn(string prefab, Vector3 position, Quaternion rotation, short syncId)
        {
            // Load the resource
            var resource = Resources.Load<GameObject>(prefab);

            if (resource != null)
            {
                // Instantiate game object
                var go = Instantiate(resource, position, rotation);

                // Get component king sync
                var KingSync = go.GetComponent<KingSync>();

                if (KingSync != null)
                {
                    // Set params
                    KingSync.SyncId = syncId;
                    KingSync.Prefab = prefab;

                    // Add king sync
                    _syncsById.Add(syncId, KingSync);
                    _syncs.Add(KingSync);

                    return KingSync;
                }

                // KingSync not present
                else
                {
                    // Destroy game object
                    Destroy(gameObject);

                    Debug.LogErrorFormat(
                        "Could not spawn '{0}' because it does not contain any KingSync component",
                        prefab
                    );
                }
            }

            // Resource is not present
            else Debug.LogErrorFormat(
                "Ressource '{0}' is not present",
                prefab
            );

            return null;
        }


        /// <summary>Unspawn by sync identifier</summary>
        /// <param name="syncId">sync identifier</param>
        public void Unspawn(short syncId)
        {
            if (!_syncsById.ContainsKey(syncId)) return;
            Unspawn(_syncsById[syncId]);
        }

        /// <summary>Unspawn</summary>
        /// <param name="sync">sync</param>
        public void Unspawn(KingSync sync)
        {
            // Remove sync
            _syncsById.Remove(sync.SyncId);
            _syncs.Remove(sync);

            // Remove from the scope of all
            foreach (KingSync kingSync in _syncs)
                kingSync.OutOfScope(sync);

            // Destroy
            Destroy(sync.gameObject);
        }


        /// <summary>Sync update</summary>
        /// <param name="buffer">King buffer</param>
        public void SyncUpdate(KingBuffer buffer)
        {
            var packet = buffer.ReadMessagePacket<KingPacket>();

            // OUT OF SCOPE
            if (packet == KingPacket.OutOfScope)
            { OutOfScope(buffer); }

            // WITHIN SCOPE
            if (packet == KingPacket.WithinScope)
            { WithinScope(buffer); }

            // SYNC SCENE
            if (packet == KingPacket.SyncScene)
            {
                // As long as you can read
                while (buffer.CanRead())
                {
                    // sync id
                    var syncId = buffer.ReadShort();

                    // Has synchronizer
                    if (!_syncsById.ContainsKey(syncId)) break;

                    // Catching
                    _syncsById[syncId].Catching(buffer);
                }
            }
        }


        /// <summary>Within scope</summary>
        /// <param name="buffer">King Buffer</param>
        private void WithinScope(KingBuffer buffer)
        {
            // Sync id
            var syncId = buffer.ReadShort();

            // Has synchronizer
            if (_syncsById.ContainsKey(syncId)) return;

            // Spawn sync
            var KingSync = Spawn(
                buffer.ReadString(),
                buffer.ReadVector3(),
                buffer.ReadQuaternion(),
                syncId
            );

            // Invoke on reading
            KingSync.InvokeOnReading(buffer);

            // Spawning
            OnSpawning.Invoke(KingSync);
        }

        /// <summary>Out of scope</summary>
        /// <param name="buffer">King Buffer</param>
        private void OutOfScope(KingBuffer buffer)
        {
            // Sync id
            var syncId = buffer.ReadShort();

            // No synchronizer
            if (!_syncsById.ContainsKey(syncId)) return;

            // Unspawning
            OnUnpawning.Invoke(_syncsById[syncId]);

            // Unspawn
            Unspawn(syncId);
        }

        /// <summary>Next sync identifier</summary>
        /// <returns>sync identifier</returns>
        private short NextsyncId()
        {
            // Next sync ID
            SyncId += 1;
            SyncId %= 0x7FFF;

            // ID 0 is used only by the owner
            if (SyncId == 0) SyncId = 1;

            return SyncId;
        }


        #region UNITY
        /// <summary>
        /// Unity UPDATE
        /// </summary>
        void Update()
        {
            // Do nothing if there is no spawn
            if (!HasSpawn) return;

            // Do nothing until the next synchronization
            if (SyncTime > Time.realtimeSinceStartup) return;
            SyncTime = Time.realtimeSinceStartup + (1f / SyncRate);

            // Synchronize and update scope
            for (int i = 0; i < _syncs.Count; i++)
            {
                // Updates the scope
                for (int j = i; j < _syncs.Count; j++)
                {
                    // Distance between two syncs
                    var dist = Vector3.Distance(
                        _syncs[i].transform.position,
                        _syncs[j].transform.position
                    );

                    // Within the scope area
                    if (dist <= ScopeRadius)
                    {
                        // Ignores if neither one has an owner
                        if (
                            _syncs[i].Owner == null &&
                            _syncs[j].Owner == null
                        ) continue;

                        // Within scope
                        _syncs[i].WithinScope(_syncs[j]);
                        _syncs[j].WithinScope(_syncs[i]);
                    }

                    // Outside the scope area
                    else
                    {
                        // Ignore if not out of range
                        // Reach here is a bit higher to avoid a sudden return
                        if (dist <= (ScopeRadius + 50)) continue;

                        // Out of scope
                        _syncs[i].OutOfScope(_syncs[j]);
                        _syncs[j].OutOfScope(_syncs[i]);
                    }
                }

                // Ignore if you do not have an owner
                if (_syncs[i].Owner == null) continue;

                // SYNC SCENE
                var buffer = new KingBuffer();
                buffer.WriteMessagePacket(KingPacket.SyncScene);

                // Putting to buffer
                _syncs[i].Putting(buffer);

                // Send to owner
                _syncs[i].Owner.Send(buffer);
            }
        }

        /// <summary>
        /// Unity AWAKE
        /// </summary>
        void Awake()
        {
            // Syncs by identifier
            _syncsById = new Dictionary<short, KingSync>();

            // Syncs
            _syncs = new List<KingSync>();
        }
        #endregion
    };
};