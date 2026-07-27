using Domain.Entities;

namespace DndCompanion.Domain.Tests.Entities;

public class ItemTests
{
    public class Create
    {
        [Fact]
        public void Throws_WhenCharacterIdEmpty()
        {
            Assert.Throws<ArgumentException>(() => 
                Item.Create(Guid.Empty, "Test Item"));
        }
        
        [Fact]
        public void Throws_WhenNameEmpty()
        {
            Assert.Throws<ArgumentException>(() => 
                Item.Create(Guid.NewGuid(), ""));
        }

        [Fact]
        public void Throws_WhenNameTooLong()
        {
            Assert.Throws<ArgumentException>(() => 
                Item.Create(Guid.NewGuid(), new string('a', 256)));
        }
        
        [Fact]
        public void Throws_WhenQuantityNegative()
        {
            Assert.Throws<ArgumentException>(() => 
                Item.Create(Guid.NewGuid(), "Test Item", quantity: -1));
        }

        [Fact]
        public void Creates_WhenValidWithDefaults()
        {
            var item = Item.Create(Guid.NewGuid(), "Sword");
            Assert.Equal("Sword", item.Name);
            Assert.Equal(1, item.Quantity);
            Assert.Null(item.Description);
            Assert.Null(item.SourceUrl);
            Assert.Null(item.ChargeResourceId);
        }
        
        [Fact]
        public void TrimsName()
        {
            var item = Item.Create(Guid.NewGuid(), "  Sword  ");
            Assert.Equal("Sword", item.Name);
        }
    }

    public class Update()
    {
        [Fact]
        public void Throws_WhenNameTooLong()
        {
            var item = Item.Create(Guid.NewGuid(), "Sword");
            Assert.Throws<ArgumentException>(() => 
                item.Update(name: new string('a', 256)));
        }

        [Fact]
        public void Throws_WhenNameIsEmpty()
        {
            var item = Item.Create(Guid.NewGuid(), "Sword");
            Assert.Throws<ArgumentException>(() => 
                item.Update(name: ""));
        }
        
        [Fact]
        public void Throws_WhenQuantityNegative()
        {
            var item = Item.Create(Guid.NewGuid(), "Sword");
            Assert.Throws<ArgumentException>(() => 
                item.Update(quantity: -1));
        }

        [Fact]
        public void Updates_WhenValid()
        {
            var item = Item.Create(Guid.NewGuid(), "Sword");
            item.Update(name: "Shield");
            Assert.Equal("Shield", item.Name);
        }

        [Fact]
        public void Updates_OnlyProvided()
        {
            var item = Item.Create(Guid.NewGuid(), "Sword");
            item.Update(name: "Shield", description: "A sturdy shield", quantity: 2);
            Assert.Equal("Shield", item.Name);
            Assert.Equal(2, item.Quantity);
            Assert.Equal("A sturdy shield", item.Description);
            Assert.Null(item.SourceUrl);
            Assert.Null(item.ChargeResourceId);
        }
    }
}