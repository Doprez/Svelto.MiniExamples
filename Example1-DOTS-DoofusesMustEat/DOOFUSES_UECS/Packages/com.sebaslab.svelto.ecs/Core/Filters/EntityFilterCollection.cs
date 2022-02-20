﻿using System.Runtime.CompilerServices;
using Svelto.DataStructures;
using Svelto.DataStructures.Native;
using Svelto.ECS.Internal;

namespace Svelto.ECS
{
    public readonly struct EntityFilterCollection
    {
        internal EntityFilterCollection(RefWrapperType refType, EnginesRoot enginesRoot)
        {
            _enginesRoot     = enginesRoot;
            _refType         = refType;
            _filtersPerGroup = new FasterDictionary<ExclusiveGroupStruct, GroupFilters>();
        }

        public EntityFilterIterator iterator => new EntityFilterIterator(this);

        public void AddEntity(EGID egid)
        {
            ITypeSafeDictionary dictionary = _enginesRoot._groupsPerEntity[_refType][egid.groupID];

            AddEntity(egid, dictionary.GetIndex(egid.entityID));
        }

        public void RemoveEntity(EGID egid)
        {
            _filtersPerGroup[egid.groupID].Remove(egid.entityID);
        }

        public void Clear()
        {
            var filterSets = _filtersPerGroup.GetValues(out var count);
            for (var i = 0; i < count; i++)
            {
                filterSets[i].Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddEntity(EGID egid, uint toIndex)
        {
            if (_filtersPerGroup.TryGetValue(egid.groupID, out var groupFilter) == false)
            {
                groupFilter                    = new GroupFilters(32, egid.groupID);
                _filtersPerGroup[egid.groupID] = groupFilter;
            }

            groupFilter.Add(egid.entityID, toIndex);
        }

        internal int groupCount => _filtersPerGroup.count;

        internal GroupFilters GetGroup(int indexGroup)
        {
            DBC.ECS.Check.Require(indexGroup < _filtersPerGroup.count);
            return _filtersPerGroup.GetValues(out _)[indexGroup];
        }

        internal void Dispose()
        {
            var filterSets = _filtersPerGroup.GetValues(out var count);
            for (var i = 0; i < count; i++)
            {
                filterSets[i].Dispose();
            }
        }

        readonly          EnginesRoot                                          _enginesRoot;
        readonly          RefWrapperType                                       _refType;
        internal readonly FasterDictionary<ExclusiveGroupStruct, GroupFilters> _filtersPerGroup;

        internal struct GroupFilters
        {
            internal GroupFilters(uint size, ExclusiveGroupStruct group)
            {
                _entityIDToDenseIndex = new SharedSveltoDictionaryNative<uint, uint>(size);
                _indexToEntityId      = new SharedSveltoDictionaryNative<uint, uint>(size);
                _group                = group;
            }

            internal void Add(uint entityId, uint entityIndex)
            {
                _entityIDToDenseIndex.Add(entityId, entityIndex);
                _indexToEntityId.Add(entityIndex, entityId);
            }

            internal void Remove(uint entityId)
            {
                _indexToEntityId.Remove(_entityIDToDenseIndex[entityId]);
                _entityIDToDenseIndex.Remove(entityId);
            }

            internal void RemoveWithSwapBack(uint entityId, uint entityIndex, uint lastIndex)
            {
                // Check if the last index is part of the filter as an entity, in that case
                //we need to update the filter
                if (entityIndex != lastIndex && _indexToEntityId.ContainsKey(lastIndex))
                {
                    uint lastEntityID = _indexToEntityId[lastIndex];

                    _entityIDToDenseIndex[lastEntityID] = entityIndex;
                    _indexToEntityId[entityIndex]       = lastEntityID;

                    _indexToEntityId.Remove(lastIndex);
                }
                else
                {
                    // We don't need to check if the entityIndex is a part of the dictionary.
                    // The Remove function will check for us.
                    _indexToEntityId.Remove(entityIndex);
                }

                // We don't need to check if the entityID is part of the dictionary.
                // The Remove function will check for us.
                _entityIDToDenseIndex.Remove(entityId);
            }

            internal void Clear()
            {
                _indexToEntityId.FastClear();
                _entityIDToDenseIndex.FastClear();
            }

            internal bool HasEntity(uint entityId) => _entityIDToDenseIndex.ContainsKey(entityId);

            internal void Dispose()
            {
                _entityIDToDenseIndex.Dispose();
                _indexToEntityId.Dispose();
            }

            internal EntityFilterIndices indices
            {
                get
                {
                    var values = _entityIDToDenseIndex.GetValues(out var count);
                    return new EntityFilterIndices(values, count);
                }
            }

            internal uint count => (uint)_entityIDToDenseIndex.count;

            internal ExclusiveGroupStruct group => _group;

            SharedSveltoDictionaryNative<uint, uint> _indexToEntityId;
            SharedSveltoDictionaryNative<uint, uint> _entityIDToDenseIndex;
            readonly ExclusiveGroupStruct            _group;
        }
    }
}