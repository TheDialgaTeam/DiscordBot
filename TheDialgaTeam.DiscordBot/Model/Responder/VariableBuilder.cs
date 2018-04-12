using Discord.Commands;

namespace TheDialgaTeam.DiscordBot.Model.Responder
{
    public interface IVariableBuilder
    {
    }

    internal sealed class VariableBuilder : IVariableBuilder
    {
        public VariableBuilder(ICommandContext context, string formattedResponse, string[] argsObjects)
        {
            // Statements:
            // {if (condition)}
            // {else if (condition)}
            // {else}
            // {end if}
            //
            // {switch (condition)}
            // {case (result)}
            // {break}
            // {default}
            // {end switch}

            // Supported Format:
            // {User.AvatarId} - Gets the id of this user's avatar.
            // {User.AvatarUrl} - Gets the url to this user's avatar.
            // {User.Discriminator} - Gets the per-username unique id for this user.
            // {User.DiscriminatorValue} - Gets the per-username unique id for this user.
            // {User.Username} - Gets the username for this user.
            // {User.CreatedAt}
            // {User.Id} - Gets the unique identifier for this object.
            // {User.Mention} - Returns a special string used to mention this object.
            // {User.Activity.Name} - Gets the activity this user is currently doing.
            // {User.Activity.Type} - Gets the activity this user is currently doing.
            // {User.Status} - Gets the current status of this user.

            // {GuildUser.JoinedAt} - Gets when this user joined this guild.
            // {GuildUser.Nickname} - Gets the nickname for this user.
            // {GuildUser.GuildPermissions.RawValue} - Gets a packed value representing all the permissions in this GuildPermissions.
            // {GuildUser.GuildPermissions.CreateInstantInvite}
            // {GuildUser.GuildPermissions.BanMembers}
            // {GuildUser.GuildPermissions.KickMembers}
            // {GuildUser.GuildPermissions.Administrator}
            // {GuildUser.GuildPermissions.ManageChannels}
            // {GuildUser.GuildPermissions.ManageGuild}
            // {GuildUser.GuildPermissions.AddReactions}
            // {GuildUser.GuildPermissions.ViewAuditLog}
            // {GuildUser.GuildPermissions.ReadMessages}
            // {GuildUser.GuildPermissions.SendMessages}
            // {GuildUser.GuildPermissions.SendTTSMessages}
            // {GuildUser.GuildPermissions.ManageMessages}
            // {GuildUser.GuildPermissions.EmbedLinks}
            // {GuildUser.GuildPermissions.AttachFiles}
            // {GuildUser.GuildPermissions.ReadMessageHistory}
            // {GuildUser.GuildPermissions.MentionEveryone}
            // {GuildUser.GuildPermissions.UseExternalEmojis}
            // {GuildUser.GuildPermissions.Connect}
            // {GuildUser.GuildPermissions.Speak}
            // {GuildUser.GuildPermissions.MuteMembers}
            // {GuildUser.GuildPermissions.DeafenMembers}
            // {GuildUser.GuildPermissions.MoveMembers}
            // {GuildUser.GuildPermissions.UseVAD}
            // {GuildUser.GuildPermissions.ChangeNickname}
            // {GuildUser.GuildPermissions.ManageNicknames}
            // {GuildUser.GuildPermissions.ManageRoles}
            // {GuildUser.GuildPermissions.ManageWebhooks}
            // {GuildUser.GuildPermissions.ManageEmojis}
            // {GuildUser.RoleIds} - Returns a collection of the ids of the roles this user is a member of in this guild, including the guild's @everyone role.
            // {GuildUser.IsDeafend}
            // {GuildUser.IsMuted}
            // {GuildUser.IsSelfDeafened}
            // {GuildUser.IsSelfMuted}
            // {GuildUser.IsSuppressed}
        }
    }
}