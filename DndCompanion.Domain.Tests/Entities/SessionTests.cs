using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace DndCompanion.Domain.Tests.Entities;

public class SessionTests
{
    private static Session CreateSession(string? pin = null) => 
        Session.Create(Guid.NewGuid(), "Master", pin is null ? null : PinCode.From(pin));
    
    public class Create
    {
        [Fact]
        public void CreatesSession_WithMasterParticipant()
        {
            var session = CreateSession();
            Assert.Single(session.Participants);
        }

        [Fact]
        public void Throws_WhenMasterDisplayNameEmpty()
        {
            Assert.Throws<ArgumentException>(() => 
                Session.Create(Guid.NewGuid(), "", null));
        }
        
        [Fact]
        public void GeneratesInviteCode_WhenCreated()
        {
            var session = CreateSession();
            Assert.NotNull(session.InviteCode);
        }
        
        [Fact]
        public void FirstParticipantIsMaster_WhenSessionCreated()
        {
            var session = CreateSession();
            var master = session.Participants.Single();
            Assert.Equal(SessionRole.Master, master.Role);
        }
    }
    
    public class Join
    {
        [Fact]
        public void AddsParticipants_WhenValid()
        {
            var session = CreateSession();
            session.Join("Player1");
            Assert.Equal(2, session.Participants.Count);
        }
        
        [Fact]
        public void Throws_WhenPinCodeRequiredAndNotProvided()
        {
            var session = CreateSession("1234");
            Assert.Throws<ArgumentException>(() => session.Join("Player1"));
        }

        [Fact]
        public void Throws_WhenPinCodeInvalid()
        {
            var session = CreateSession("1234");
            Assert.Throws<ArgumentException>(() => 
                session.Join("Player1", pinCode: PinCode.From("123")));
        }
        
        [Fact]
        public void Joins_WhenPinCodeValid()
        {
            var session = CreateSession("1234");
            session.Join("Player1", pinCode: PinCode.From("1234"));
            Assert.Equal(2, session.Participants.Count);
        }
        
        [Fact]
        public void Throws_WhenUserAlreadyJoined()
        {
            var session = CreateSession();
            var userId = Guid.NewGuid();
            session.Join("Player1", userId);
            Assert.Throws<ArgumentException>(() => session.Join("Player1", userId));
        }
        
        [Fact]
        public void Throws_WhenDisplayNameEmpty()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.Join(""));
        }

        [Fact]
        public void TrimsDisplayName_WhenValid()
        {
            var session = CreateSession();
            var participant = session.Join("  Player1  ");
            Assert.Equal("Player1", participant.DisplayName);
        }

        [Fact]
        public void AllowsMultipleAnonymousJoins_WhenNoUserIdProvided()
        {
            var session = CreateSession();
            session.Join("Player1");
            session.Join("Player2");
            Assert.Equal(3, session.Participants.Count);
        }
    }
}