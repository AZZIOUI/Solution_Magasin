using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solution_Magasin.Migrations.DotnetProject
{
    /// <inheritdoc />
    public partial class AddArticleImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorie",
                columns: table => new
                {
                    id_cat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_cat = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    description_cat = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__D54686DEFF867280", x => x.id_cat);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    id_client = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_client = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    prenom_client = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    adresse_client = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    mail_client = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    tel_client = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Client__6EC2B6C07F78BFA5", x => x.id_client);
                });

            migrationBuilder.CreateTable(
                name: "Employe",
                columns: table => new
                {
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CIN = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    prenom_emp = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    nom_emp = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    dateEmbauche = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employe__1A4FA5B827B60C55", x => x.id_utilisateur);
                });

            migrationBuilder.CreateTable(
                name: "Fournisseur",
                columns: table => new
                {
                    id_fourni = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CIN = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    nom_fourni = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    prenom_fourni = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    adresse_fourni = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    tel_fourni = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    mail_fourni = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Fourniss__035DEECF4FD13219", x => x.id_fourni);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id_payment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    methode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    date_payment = table.Column<DateTime>(type: "datetime", nullable: true),
                    estPaye = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__862FEFE014B98D38", x => x.id_payment);
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    id_article = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reference_art = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    nom_art = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    designation_art = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    prix_unit = table.Column<double>(type: "float", nullable: true),
                    date_ajout = table.Column<DateOnly>(type: "date", nullable: true),
                    id_cat = table.Column<int>(type: "int", nullable: true),
                    image_path = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Article__64CB31B8FDBA7DD5", x => x.id_article);
                    table.ForeignKey(
                        name: "FK__Article__id_cat__44FF419A",
                        column: x => x.id_cat,
                        principalTable: "Categorie",
                        principalColumn: "id_cat");
                });

            migrationBuilder.CreateTable(
                name: "Presence",
                columns: table => new
                {
                    id_pr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    dateP = table.Column<DateOnly>(type: "date", nullable: true),
                    heure_arrive = table.Column<TimeOnly>(type: "time", nullable: true),
                    heure_depart = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Presence__0148A34E4E909523", x => x.id_pr);
                    table.ForeignKey(
                        name: "FK__Presence__id_uti__403A8C7D",
                        column: x => x.id_utilisateur,
                        principalTable: "Employe",
                        principalColumn: "id_utilisateur");
                });

            migrationBuilder.CreateTable(
                name: "Achat",
                columns: table => new
                {
                    id_achat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date_achat = table.Column<DateTime>(type: "datetime", nullable: true),
                    total_ach = table.Column<double>(type: "float", nullable: true),
                    id_fourni = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Achat__ED270182E8DFCDB2", x => x.id_achat);
                    table.ForeignKey(
                        name: "FK__Achat__id_fourni__4D94879B",
                        column: x => x.id_fourni,
                        principalTable: "Fournisseur",
                        principalColumn: "id_fourni");
                });

            migrationBuilder.CreateTable(
                name: "Vente",
                columns: table => new
                {
                    id_vente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date_vente = table.Column<DateTime>(type: "datetime", nullable: true),
                    total_v = table.Column<double>(type: "float", nullable: true),
                    adresse_liv = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    id_client = table.Column<int>(type: "int", nullable: false),
                    id_payment = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vente__459533B319319C7A", x => x.id_vente);
                    table.ForeignKey(
                        name: "FK__Vente__id_client__5629CD9C",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "id_client");
                    table.ForeignKey(
                        name: "FK__Vente__id_paymen__571DF1D5",
                        column: x => x.id_payment,
                        principalTable: "Payment",
                        principalColumn: "id_payment");
                });

            migrationBuilder.CreateTable(
                name: "NotificationStock",
                columns: table => new
                {
                    id_not = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_article = table.Column<int>(type: "int", nullable: false),
                    msg = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    date_not = table.Column<DateTime>(type: "datetime", nullable: true),
                    vu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__6E4C7671050D7F42", x => x.id_not);
                    table.ForeignKey(
                        name: "FK__Notificat__id_ar__4AB81AF0",
                        column: x => x.id_article,
                        principalTable: "Article",
                        principalColumn: "id_article");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    id_review = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comment = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    id_client = table.Column<int>(type: "int", nullable: false),
                    id_article = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__2F79F8C7912B0A56", x => x.id_review);
                    table.ForeignKey(
                        name: "FK__Review__id_artic__5EBF139D",
                        column: x => x.id_article,
                        principalTable: "Article",
                        principalColumn: "id_article");
                    table.ForeignKey(
                        name: "FK__Review__id_clien__5DCAEF64",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "id_client");
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    id_st = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stockmax = table.Column<int>(type: "int", nullable: true),
                    stockmin = table.Column<int>(type: "int", nullable: true),
                    qte_dispo = table.Column<int>(type: "int", nullable: true),
                    date_modification = table.Column<DateOnly>(type: "date", nullable: true),
                    id_article = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Stock__014858EB29631BEF", x => x.id_st);
                    table.ForeignKey(
                        name: "FK__Stock__id_articl__47DBAE45",
                        column: x => x.id_article,
                        principalTable: "Article",
                        principalColumn: "id_article");
                });

            migrationBuilder.CreateTable(
                name: "DetailAchat",
                columns: table => new
                {
                    id_da = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    qte_da = table.Column<int>(type: "int", nullable: true),
                    montant_da = table.Column<double>(type: "float", nullable: true),
                    id_achat = table.Column<int>(type: "int", nullable: false),
                    id_article = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetailAc__00B7C6E67393A75E", x => x.id_da);
                    table.ForeignKey(
                        name: "FK__DetailAch__id_ac__5070F446",
                        column: x => x.id_achat,
                        principalTable: "Achat",
                        principalColumn: "id_achat");
                    table.ForeignKey(
                        name: "FK__DetailAch__id_ar__5165187F",
                        column: x => x.id_article,
                        principalTable: "Article",
                        principalColumn: "id_article");
                });

            migrationBuilder.CreateTable(
                name: "DetailVente",
                columns: table => new
                {
                    id_dv = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    qte_dv = table.Column<int>(type: "int", nullable: true),
                    montant_dv = table.Column<double>(type: "float", nullable: true),
                    id_vente = table.Column<int>(type: "int", nullable: false),
                    id_article = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetailVe__00B7C6CD84831A7A", x => x.id_dv);
                    table.ForeignKey(
                        name: "FK__DetailVen__id_ar__5AEE82B9",
                        column: x => x.id_article,
                        principalTable: "Article",
                        principalColumn: "id_article");
                    table.ForeignKey(
                        name: "FK__DetailVen__id_ve__59FA5E80",
                        column: x => x.id_vente,
                        principalTable: "Vente",
                        principalColumn: "id_vente");
                });

            migrationBuilder.CreateTable(
                name: "Facture",
                columns: table => new
                {
                    id_facture = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_facture = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    date_facture = table.Column<DateOnly>(type: "date", nullable: true),
                    montant_total = table.Column<double>(type: "float", nullable: true),
                    file_path = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    id_vente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Facture__6C08ED575425E103", x => x.id_facture);
                    table.ForeignKey(
                        name: "FK__Facture__id_vent__619B8048",
                        column: x => x.id_vente,
                        principalTable: "Vente",
                        principalColumn: "id_vente");
                });

            migrationBuilder.CreateTable(
                name: "Retour",
                columns: table => new
                {
                    id_retour = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    motif = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    date_retour = table.Column<DateTime>(type: "datetime", nullable: true),
                    id_article = table.Column<int>(type: "int", nullable: false),
                    id_vente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Retour__4E89E94F61945E7A", x => x.id_retour);
                    table.ForeignKey(
                        name: "FK__Retour__id_artic__6477ECF3",
                        column: x => x.id_article,
                        principalTable: "Article",
                        principalColumn: "id_article");
                    table.ForeignKey(
                        name: "FK__Retour__id_vente__656C112C",
                        column: x => x.id_vente,
                        principalTable: "Vente",
                        principalColumn: "id_vente");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achat_id_fourni",
                table: "Achat",
                column: "id_fourni");

            migrationBuilder.CreateIndex(
                name: "IX_Article_id_cat",
                table: "Article",
                column: "id_cat");

            migrationBuilder.CreateIndex(
                name: "IX_DetailAchat_id_achat",
                table: "DetailAchat",
                column: "id_achat");

            migrationBuilder.CreateIndex(
                name: "IX_DetailAchat_id_article",
                table: "DetailAchat",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_DetailVente_id_article",
                table: "DetailVente",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_DetailVente_id_vente",
                table: "DetailVente",
                column: "id_vente");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_id_vente",
                table: "Facture",
                column: "id_vente");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationStock_id_article",
                table: "NotificationStock",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_Presence_id_utilisateur",
                table: "Presence",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Retour_id_article",
                table: "Retour",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_Retour_id_vente",
                table: "Retour",
                column: "id_vente");

            migrationBuilder.CreateIndex(
                name: "IX_Review_id_article",
                table: "Review",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_Review_id_client",
                table: "Review",
                column: "id_client");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_id_article",
                table: "Stock",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_Vente_id_client",
                table: "Vente",
                column: "id_client");

            migrationBuilder.CreateIndex(
                name: "IX_Vente_id_payment",
                table: "Vente",
                column: "id_payment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailAchat");

            migrationBuilder.DropTable(
                name: "DetailVente");

            migrationBuilder.DropTable(
                name: "Facture");

            migrationBuilder.DropTable(
                name: "NotificationStock");

            migrationBuilder.DropTable(
                name: "Presence");

            migrationBuilder.DropTable(
                name: "Retour");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "Achat");

            migrationBuilder.DropTable(
                name: "Employe");

            migrationBuilder.DropTable(
                name: "Vente");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Fournisseur");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Categorie");
        }
    }
}
