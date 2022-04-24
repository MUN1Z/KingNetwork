using Xunit;
using XUnitPriorityOrderer;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer(CollectionPriorityOrderer.TypeName, CollectionPriorityOrderer.AssembyName)]