CREATE TABLE Role (
    id_role INT PRIMARY KEY IDENTITY(1,1),
    role VARCHAR(50) NOT NULL
);

CREATE TABLE Client (
    id_client INT PRIMARY KEY IDENTITY(1,1),
    nom_client VARCHAR(100),
    prenom_client VARCHAR(100),
    adresse_client VARCHAR(200),
    mail_client VARCHAR(200),
    tel_client VARCHAR(50),
    pwd_client VARCHAR(200)
);

CREATE TABLE Fournisseur (
    id_fourni INT PRIMARY KEY IDENTITY(1,1),
    CIN VARCHAR(30),
    nom_fourni VARCHAR(100),
    prenom_fourni VARCHAR(100),
    adresse_fourni VARCHAR(200),
    tel_fourni VARCHAR(50),
    mail_fourni VARCHAR(200)
);

CREATE TABLE Employe (
    id_utilisateur INT PRIMARY KEY IDENTITY(1,1),
    CIN VARCHAR(30),
    prenom_emp VARCHAR(100),
    nom_emp VARCHAR(100),
    login VARCHAR(100),
    pwd VARCHAR(200),
    dateEmbauche DATE,
    id_role INT NOT NULL,
    FOREIGN KEY (id_role) REFERENCES Role(id_role)
);

CREATE TABLE Presence (
    id_pr INT PRIMARY KEY IDENTITY(1,1),
    id_utilisateur INT NOT NULL,
    dateP DATE,
    heure_arrive TIME,
    heure_depart TIME,
    FOREIGN KEY (id_utilisateur) REFERENCES Employe(id_utilisateur)
);

CREATE TABLE Categorie (
    id_cat INT PRIMARY KEY IDENTITY(1,1),
    nom_cat VARCHAR(100),
    description_cat VARCHAR(200)
);

CREATE TABLE Article (
    id_article INT PRIMARY KEY IDENTITY(1,1),
    reference_art VARCHAR(100),
    nom_art VARCHAR(100),
    designation_art VARCHAR(200),
    prix_unit FLOAT,
    date_ajout DATE,
    id_cat INT,
    FOREIGN KEY (id_cat) REFERENCES Categorie(id_cat)
);

CREATE TABLE Stock (
    id_st INT PRIMARY KEY IDENTITY(1,1),
    stockmax INT,
    stockmin INT,
    qte_dispo INT,
    date_modification DATE,
    id_article INT NOT NULL,
    FOREIGN KEY (id_article) REFERENCES Article(id_article)
);

CREATE TABLE NotificationStock (
    id_not INT PRIMARY KEY IDENTITY(1,1),
    id_article INT NOT NULL,
    msg VARCHAR(200),
    date_not DATETIME,
    vu BIT,
    FOREIGN KEY (id_article) REFERENCES Article(id_article)
);

CREATE TABLE Achat (
    id_achat INT PRIMARY KEY IDENTITY(1,1),
    date_achat DATETIME,
    total_ach DOUBLE PRECISION,
    id_fourni INT NOT NULL,
    FOREIGN KEY (id_fourni) REFERENCES Fournisseur(id_fourni)
);

CREATE TABLE DetailAchat (
    id_da INT PRIMARY KEY IDENTITY(1,1),
    qte_da INT,
    montant_da FLOAT,
    id_achat INT NOT NULL,
    id_article INT NOT NULL,
    FOREIGN KEY (id_achat) REFERENCES Achat(id_achat),
    FOREIGN KEY (id_article) REFERENCES Article(id_article)
);

CREATE TABLE Payment (
    id_payment INT PRIMARY KEY IDENTITY(1,1),
    methode VARCHAR(50),
    date_payment DATETIME,
    estPaye BIT
);

CREATE TABLE Vente (
    id_vente INT PRIMARY KEY IDENTITY(1,1),
    date_vente DATETIME,
    total_v DOUBLE PRECISION,
    adresse_liv VARCHAR(200),
    status VARCHAR(50),
    id_client INT NOT NULL,
    id_payment INT,
    FOREIGN KEY (id_client) REFERENCES Client(id_client),
    FOREIGN KEY (id_payment) REFERENCES Payment(id_payment)
);

CREATE TABLE DetailVente (
    id_dv INT PRIMARY KEY IDENTITY(1,1),
    qte_dv INT,
    montant_dv FLOAT,
    id_vente INT NOT NULL,
    id_article INT NOT NULL,
    FOREIGN KEY (id_vente) REFERENCES Vente(id_vente),
    FOREIGN KEY (id_article) REFERENCES Article(id_article)
);

CREATE TABLE Review (
    id_review INT PRIMARY KEY IDENTITY(1,1),
    comment VARCHAR(300),
    rating INT,
    id_client INT NOT NULL,
    id_article INT NOT NULL,
    FOREIGN KEY (id_client) REFERENCES Client(id_client),
    FOREIGN KEY (id_article) REFERENCES Article(id_article)
);

CREATE TABLE Facture (
    id_facture INT PRIMARY KEY IDENTITY(1,1),
    code_facture VARCHAR(100),
    date_facture DATE,
    montant_total DOUBLE PRECISION,
    file_path VARCHAR(300),
    id_vente INT NOT NULL,
    FOREIGN KEY (id_vente) REFERENCES Vente(id_vente)
);

CREATE TABLE Retour (
    id_retour INT PRIMARY KEY IDENTITY(1,1),
    motif VARCHAR(200),
    date_retour DATETIME,
    id_article INT NOT NULL,
    id_vente INT NOT NULL,
    FOREIGN KEY (id_article) REFERENCES Article(id_article),
    FOREIGN KEY (id_vente) REFERENCES Vente(id_vente)
);
