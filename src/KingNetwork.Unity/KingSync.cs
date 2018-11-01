using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using UnityEngine;
using System.Collections.Generic;
using System;


namespace KingNetwork.Unity {
	/// <summary>
	/// King Sync.
	/// This class is responsible for synchronizing objects in the scene.
	/// </summary>
	[
		AddComponentMenu("KingNetwork/King Sync")
	]
	public class KingSync : MonoBehaviour {
		/// <summary>Owner identifier</summary>
		internal const short OWNER_ID = 0;


		/// <summary>Syncs by identifier</summary>
		private IDictionary<short, KingSync> _syncsById;


		/// <summary>Prefab</summary>
		internal string Prefab;

		/// <summary>Sync identifier</summary>
		internal short SyncId;


		/// <summary>Occurs when on writing</summary>
		public Action<KingBuffer, bool> OnWriting;

		/// <summary>Occurs when on reading</summary>
		public Action<KingBuffer> OnReading;

		/// <summary>Owner</summary>
		public IClient Owner;


		/// <summary>Clear<summary>
		public void Clear()
		{_syncsById.Clear();}


		/// <summary>Putting stream</summary>
		/// <param name="buffer">King Buffer</param>
		internal void Putting(KingBuffer buffer){
			// Serialize all of the scope
			foreach(var syncPair in _syncsById){
				// Write node identifier
				// Owner always receives ID 0
				if(syncPair.Value == this) buffer.WriteShort(OWNER_ID);
				else buffer.WriteShort(syncPair.Value.SyncId);

				// On writing
				syncPair.Value.InvokeOnWriting(buffer, false);
			}
		}

		/// <summary>Catching buffer</summary>
		/// <param name="buffer">King Buffer</param>
		internal void Catching(KingBuffer buffer)
		{OnReading.Invoke(buffer);}


		/// <summary>Within scope</summary>
		/// <param name="sync">King Sync</param>
		internal void WithinScope(KingSync sync){
			// Ignore if there is no owner
			if(Owner == null) return;

			// Ignore if in scope
			if(_syncsById.ContainsKey(sync.SyncId)) return;

			// Add to indexed sync
			_syncsById.Add(sync.SyncId, sync);

			// WITHIN SCOPE
			var buffer = new KingBuffer();
			buffer.WriteMessagePacket(KingPacket.WithinScope);

			// Write node identifier
			// Owner always receives ID 0
			if(sync == this) buffer.WriteShort(OWNER_ID);
			else buffer.WriteShort(sync.SyncId);

			// Write prefab
			buffer.WriteString(Prefab);

			// Write full data
			InvokeOnWriting(buffer, true);

			// Send
			Owner.Send(buffer);
		}

		/// <summary>Out of scope</summary>
		/// <param name="sync">King Sync</param>
		internal void OutOfScope(KingSync sync){
			// Ignore if there is no owner
			if(Owner == null) return;

			// Ignore if not in scope
			if(!_syncsById.ContainsKey(sync.SyncId)) return;

			// Remove from indexed sync
			_syncsById.Remove(sync.SyncId);

			// OUT OF SCOPE
			var buffer = new KingBuffer(); ;
			buffer.WriteMessagePacket(KingPacket.OutOfScope);

			// Write node identifier
			// Owner always receives ID 0
			if(sync == this) buffer.WriteShort(OWNER_ID);
			else buffer.WriteShort(sync.SyncId);

			// Send
			Owner.Send(buffer);
		}


		/// <summary>Invoke on writing</summary>
		/// <param name="buffer">King Buffer</param>
		/// <param name="full">True is full</param>
		internal void InvokeOnWriting(KingBuffer buffer, bool full)
		{OnWriting.Invoke(buffer, full);}

		/// <summary>Invoke on reading</summary>
		/// <param name="buffer">King Buffer</param>
		internal void InvokeOnReading(KingBuffer buffer)
		{OnReading.Invoke(buffer);}


		#region UNITY
		/// <summary>
		/// Unity AWAKE
		/// </summary>
		void Awake(){
			// Syncs by identifier
			_syncsById = new Dictionary<short, KingSync>();
		}
		#endregion
	};
};