using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Models;

public class NZWalksAuthDbContext : IdentityDbContext
{
    public NZWalksAuthDbContext()
    {
    }

    public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* IdentityRole에 데이터 주입하기 (add-migration시 -Context로 context 지정해야함 */

        var readerRoleId = "a71a55d6-99d7-4123-b4e0-1218ecb90e3e";
        var writerRoleId = "c309fa92-2123-47be-b397-a1c77adb502c";

        var roles = new List<IdentityRole>
        {
            new IdentityRole()
            {
                Id = readerRoleId,
                ConcurrencyStamp = readerRoleId,
                Name = "Reader",
                NormalizedName = "Reader".ToUpper()
            },
            new IdentityRole
            {
                Id = writerRoleId,
                ConcurrencyStamp = writerRoleId,
                Name = "Writer",
                NormalizedName = "Writer".ToUpper()
            }
        };

        builder.Entity<IdentityRole>().HasData(roles);

    }

}
