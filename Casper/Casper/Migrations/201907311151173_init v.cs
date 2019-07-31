namespace Alto_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initv : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exigences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IDExigence = c.Int(nullable: false),
                        Name = c.String(),
                        DocumentPath = c.String(),
                        DocumentName = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        IsNodeExpanded = c.Boolean(nullable: false),
                        ForeignKey = c.Int(nullable: false),
                        ForeignKey_TO_Norme = c.Int(nullable: false),
                        ForeignKey_TO_Projet = c.Int(nullable: false),
                        Exigence_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exigences", t => t.Exigence_Id)
                .Index(t => t.Exigence_Id);
            
            CreateTable(
                "dbo.Mesures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FK_to_Projets = c.Int(nullable: false),
                        FK_to_Mesures = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        DocumentPath = c.String(),
                        DocumentName = c.String(),
                        Status = c.Int(nullable: false),
                        IsNodeExpanded = c.Boolean(nullable: false),
                        Mesure_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mesures", t => t.Mesure_Id)
                .Index(t => t.Mesure_Id);
            
            CreateTable(
                "dbo.Normes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom_Norme = c.String(),
                        DocumentPath = c.String(),
                        DocumentName = c.String(),
                        IDNorme = c.Int(nullable: false),
                        FK_to_Projet = c.Int(nullable: false),
                        Norme_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Normes", t => t.Norme_Id)
                .Index(t => t.Norme_Id);
            
            CreateTable(
                "dbo.Projets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Provider = c.Int(nullable: false),
                        Projet_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projets", t => t.Projet_Id)
                .Index(t => t.Projet_Id);
            
            CreateTable(
                "dbo.RelationMesureExigences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdMesure = c.Int(nullable: false),
                        IdExigence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Identifiant = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projets", "Projet_Id", "dbo.Projets");
            DropForeignKey("dbo.Normes", "Norme_Id", "dbo.Normes");
            DropForeignKey("dbo.Mesures", "Mesure_Id", "dbo.Mesures");
            DropForeignKey("dbo.Exigences", "Exigence_Id", "dbo.Exigences");
            DropIndex("dbo.Projets", new[] { "Projet_Id" });
            DropIndex("dbo.Normes", new[] { "Norme_Id" });
            DropIndex("dbo.Mesures", new[] { "Mesure_Id" });
            DropIndex("dbo.Exigences", new[] { "Exigence_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.RelationMesureExigences");
            DropTable("dbo.Projets");
            DropTable("dbo.Normes");
            DropTable("dbo.Mesures");
            DropTable("dbo.Exigences");
        }
    }
}
