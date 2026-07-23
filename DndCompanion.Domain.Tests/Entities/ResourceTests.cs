using Domain.Entities;
using Domain.Enums;

namespace DndCompanion.Domain.Tests.Entities;

public class ResourceTests
{
    public class Create
    {
        [Fact]
        public void Throws_WhenCharacterIdEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                Resource.Create(
                    Guid.Empty,
                    "Resource name",
                    ResourceType.AbilitySlot,
                    10,
                    RecoveryType.LongRest
                ));
        }

        [Fact]
        public void Throws_WhenMaxValueNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                Resource.Create(
                    Guid.NewGuid(),
                    "Resource name",
                    ResourceType.AbilitySlot,
                    -1,
                    RecoveryType.LongRest
                ));
        }

        [Fact]
        public void SetsCurrentToMax_WhenInitialCurrentNotProvided()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );

            Assert.Equal(10, resource.CurrentValue);
        }

        [Fact]
        public void SetsCurrentToInitialValue_WhenProvided()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                initialCurrent: 5
            );

            Assert.Equal(5, resource.CurrentValue);
        }

        [Fact]
        public void Throws_WhenInitialCurrentNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                Resource.Create(
                    Guid.NewGuid(),
                    "Resource name",
                    ResourceType.AbilitySlot,
                    10,
                    RecoveryType.LongRest,
                    initialCurrent: -1
                ));
        }

        [Fact]
        public void Throws_WhenInitialCurrentGreaterThanMaxValue()
        {
            Assert.Throws<ArgumentException>(() =>
                Resource.Create(
                    Guid.NewGuid(),
                    "Resource name",
                    ResourceType.AbilitySlot,
                    10,
                    RecoveryType.LongRest,
                    initialCurrent: 11
                )
            );
        }
    }
    
    public class Change
    {
        [Fact]
        public void ClampsToZero_WhenDeltaExceedsCurrentValue()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );

            resource.Change(-15);

            Assert.Equal(0, resource.CurrentValue);
        }
        
        [Fact]
        public void ClampsToMaxValue_WhenDeltaExceedsMaxValue()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );

            resource.Change(15);

            Assert.Equal(10, resource.CurrentValue);
        }
        
        [Fact]
        public void IncreasesCurrentValue_WhenDeltaPositive()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                initialCurrent: 5
            );
            
            resource.Change(3);
            
            Assert.Equal(8, resource.CurrentValue);
        }
        
        [Fact]
        public void DecreasesCurrentValue_WhenDeltaNegative()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest,
                initialCurrent: 5
            );
            
            resource.Change(-3);
            
            Assert.Equal(2, resource.CurrentValue);
        }
    }
    
    public class SetMax
    {
        [Fact]
        public void Throws_WhenMaxValueNegative()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );
            
            Assert.Throws<ArgumentException>(() => resource.SetMax(-1));
        }
        
        [Fact]
        public void ReducesCurrent_WhenMaxValueLessThanCurrent()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );
            
            resource.SetMax(5);
            
            Assert.Equal(5, resource.CurrentValue);
        }
        
        [Fact]
        public void FillsToMax_WhenMaxValueGreaterThanCurrent()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );
            
            resource.SetMax(15);
            
            Assert.Equal(15, resource.CurrentValue);
        }
        
        [Fact]
        public void DoesNotChangeCurrent_WhenIfChangedFalse()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );
            
            resource.SetMax(15, false);
            
            Assert.Equal(10, resource.CurrentValue);
        }
    }
    
    public class Recover
    {
        [Fact] 
        public void ReturnsFalse_WhenRecoveryTypeIsNone()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.None
            );
            
            Assert.False(resource.CanRecoverOn(RecoveryType.LongRest));
        }
        
        [Fact]
        public void ReturnsTrue_WhenRecoveryTypeMatches()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );
            
            Assert.True(resource.CanRecoverOn(RecoveryType.LongRest));
        }
        
        [Fact]
        public void ReturnsTrue_WhenRecoveryTypeShortRestAndIncludeFlag()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.ShortRest
            );
            
            Assert.True(resource.CanRecoverOn(RecoveryType.LongRest));
        }
        
        [Fact]
        public void ReturnsFalse_WhenRecoveryTypeShortRestWithoutFlag()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.ShortRest
            );
            
            Assert.False(resource.CanRecoverOn(RecoveryType.LongRest, false));
        }
        
        [Fact]
        public void ReturnsFalse_WhenRecoveryTypeDoesNotMatch()
        {
            var resource = Resource.Create(
                Guid.NewGuid(),
                "Resource name",
                ResourceType.AbilitySlot,
                10,
                RecoveryType.LongRest
            );
            
            Assert.False(resource.CanRecoverOn(RecoveryType.ShortRest));
        }
    }
}