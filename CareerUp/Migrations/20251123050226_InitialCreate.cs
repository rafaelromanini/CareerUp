using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerUp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_usuario",
                columns: table => new
                {
                    id_usuario = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome_usuario = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    cpf = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    cargo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    papel = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_usuario", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "tb_habilidade",
                columns: table => new
                {
                    id_usuario = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    habilidade_primaria = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    habilidade_secundaria = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    habilidade_terciaria = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_habilidade", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_tb_habilidade_tb_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "tb_usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_login_usuario",
                columns: table => new
                {
                    id_usuario = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    login = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(180)", maxLength: 180, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_login_usuario", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_tb_login_usuario_tb_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "tb_usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_recomendacao",
                columns: table => new
                {
                    id_recomendacao = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    data_geracao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    resultado_ia = table.Column<string>(type: "CLOB", nullable: false),
                    id_usuario = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_recomendacao", x => x.id_recomendacao);
                    table.ForeignKey(
                        name: "FK_tb_recomendacao_tb_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "tb_usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_login_usuario",
                table: "tb_login_usuario",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_recomendacao_data",
                table: "tb_recomendacao",
                column: "data_geracao");

            migrationBuilder.CreateIndex(
                name: "idx_recomendacao_usuario",
                table: "tb_recomendacao",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_cpf",
                table: "tb_usuario",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_usuario_email",
                table: "tb_usuario",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_habilidade");

            migrationBuilder.DropTable(
                name: "tb_login_usuario");

            migrationBuilder.DropTable(
                name: "tb_recomendacao");

            migrationBuilder.DropTable(
                name: "tb_usuario");
        }
    }
}
