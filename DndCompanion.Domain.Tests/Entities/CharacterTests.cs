using Domain.Entities;
using Domain.Enums;

namespace DndCompanion.Domain.Tests.Entities;

public class CharacterTests
{
    private static Character CreateCharacter() => Character.Create("Test Character", Guid.NewGuid());
    
    public class AddResource
    {
        [Fact]
        public void AddsResource_WhenValid()
        {
            var character = CreateCharacter();
            character.AddResource(
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                "Test Resource"
            );
            
            Assert.Single(character.Resources);
            var resource = character.Resources.First();
            Assert.Equal(ResourceType.AbilitySlot, resource.Type);
            Assert.Equal("Test Resource", resource.Name);
            Assert.Equal(10, resource.MaxValue);
            Assert.Equal(10, resource.CurrentValue);
            Assert.Equal(RecoveryType.LongRest, resource.RecoveryType);
        }
        
        [Fact]
        public void Throws_WhenDuplicateTypeAndName()
        {
            var character = CreateCharacter();
            character.AddResource(
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                "Test Resource"
            );
            
            Assert.Throws<InvalidOperationException>(() => character.AddResource(
                ResourceType.AbilitySlot,
                5,
                RecoveryType.ShortRest,
                "Test Resource"
            ));
        }

        [Fact]
        public void AllowsSameType_WhenDifferentNames()
        {
            var character = CreateCharacter();
            
            character.AddResource(
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                "Resource One"
            );
            character.AddResource(
                ResourceType.AbilitySlot,
                5,
                RecoveryType.ShortRest,
                "Resource Two"
            );
            
            Assert.Equal(2, character.Resources.Count);
        }
    }
    
    public class ApplyRest
    {
        [Fact]
        public void RecoversLongRestResources_OnLongRest()
        {
            var character = CreateCharacter();
            character.AddResource(
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                "Test Resource",
                initialCurrent: 5
            );
            
            character.ApplyRest(RecoveryType.LongRest);
            
            Assert.Equal(10, character.Resources.First().CurrentValue);
        }
        
        [Fact]
        public void DoesNotRecover_WhenRestTypeMismatch()
        {
            var character = CreateCharacter();
            character.AddResource(
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                "Test Resource",
                initialCurrent: 5
            );
            
            character.ApplyRest(RecoveryType.ShortRest);
            
            Assert.Equal(5, character.Resources.First().CurrentValue);
        }

        [Fact]
        public void ReturnsAffectedCount()
        {
            var character = CreateCharacter();
            character.AddResource(
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                "Test Resource",
                initialCurrent: 5
            );
            
            character.AddResource(
                ResourceType.AbilitySlot,
                5,
                RecoveryType.ShortRest,
                "Short Rest Resource",
                initialCurrent: 2
            );
            
            character.AddResource(
                ResourceType.AbilitySlot,
                8,
                RecoveryType.None,
                "No Recovery Resource",
                initialCurrent: 3
            );
            
            var affectedCount = character.ApplyRest(RecoveryType.LongRest);
            Assert.Equal(2, affectedCount);
        }
    }

    public class AddItem
    {
        [Fact]
        public void AddsItem_WhenValid()
        {
            var character = CreateCharacter();
            character.AddItem("Sword", "A sharp blade", "http://example.com/sword");
            Assert.Single(character.Items);
        }
        
        [Fact]
        public void ReturnsItem_WhenValid()
        {
            var character = CreateCharacter();
            var item = character.AddItem("Sword", "A sharp blade", "http://example.com/sword");
            Assert.Equal("Sword", item.Name);
            Assert.Equal("A sharp blade", item.Description);
            Assert.Equal("http://example.com/sword", item.SourceUrl);
            Assert.Equal(1, item.Quantity);
            Assert.Null(item.ChargeResourceId);
        }
        
        [Fact]
        public void AddsMultipleItems_WhenValid()
        {
            var character = CreateCharacter();
            character.AddItem("Sword", "A sharp blade", "http://example.com/sword");
            character.AddItem("Shield", "A sturdy shield", "http://example.com/shield");
            Assert.Equal(2, character.Items.Count);
        }
    }
    
    public class RemoveItem
    {
        [Fact]
        public void RemovesItem_WhenValid()
        {
            var character = CreateCharacter();
            var item = character.AddItem("Sword", "A sharp blade", "http://example.com/sword");
            character.RemoveItem(item.Id);
            Assert.Empty(character.Items);
        }

        [Fact]
        public void Throws_WhenItemNotFound()
        {
            var character = CreateCharacter();
            Assert.Throws<ArgumentException>(() => character.RemoveItem(Guid.NewGuid()));
        }
    }
}