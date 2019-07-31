namespace Alto_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ini : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Exigences", "Exigence_Id", "dbo.Exigences");
            DropForeignKey("dbo.Mesures", "Mesures_Id", "dbo.Mesures");
            DropForeignKey("dbo.Normes", "Norme_Id", "dbo.Normes");
            DropIndex("dbo.Exigences", new[] { "Exigence_Id" });
            DropIndex("dbo.Mesures", new[] { "Mesures_Id" });
            DropIndex("dbo.Normes", new[] { "Norme_Id" });
            RenameColumn(table: "dbo.Projets", name: "Projets_Id", newName: "Projet_Id");
            RenameIndex(table: "dbo.Projets", name: "IX_Projets_Id", newName: "IX_Projet_Id");
            DropTable("dbo.Exigences");
            DropTable("dbo.Mesures");
            DropTable("dbo.Normes");
            DropTable("dbo.RelationMesureExigences");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
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
                        Mesures_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            RenameIndex(table: "dbo.Projets", name: "IX_Projet_Id", newName: "IX_Projets_Id");
            RenameColumn(table: "dbo.Projets", name: "Projet_Id", newName: "Projets_Id");
            CreateIndex("dbo.Normes", "Norme_Id");
            CreateIndex("dbo.Mesures", "Mesures_Id");
            CreateIndex("dbo.Exigences", "Exigence_Id");
            AddForeignKey("dbo.Normes", "Norme_Id", "dbo.Normes", "Id");
            AddForeignKey("dbo.Mesures", "Mesures_Id", "dbo.Mesures", "Id");
            AddForeignKey("dbo.Exigences", "Exigence_Id", "dbo.Exigences", "Id");
        }
    }
}
