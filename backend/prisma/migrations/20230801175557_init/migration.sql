-- CreateEnum
CREATE TYPE "UserRole" AS ENUM ('USER', 'ADMIN');

-- CreateTable
CREATE TABLE "User" (
    "id" SERIAL NOT NULL,
    "externalId" TEXT NOT NULL,

    CONSTRAINT "User_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "Organisation" (
    "id" SERIAL NOT NULL,
    "slug" TEXT NOT NULL,

    CONSTRAINT "Organisation_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "OrganisationUser" (
    "userId" INTEGER NOT NULL,
    "organisationId" INTEGER NOT NULL,
    "role" "UserRole" NOT NULL DEFAULT 'USER',

    CONSTRAINT "OrganisationUser_pkey" PRIMARY KEY ("userId","organisationId")
);

-- CreateTable
CREATE TABLE "Institution" (
    "id" SERIAL NOT NULL,
    "externalId" TEXT NOT NULL,
    "name" TEXT NOT NULL,
    "logo" TEXT NOT NULL,
    "countries" TEXT[],

    CONSTRAINT "Institution_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "InstitutionConnection" (
    "id" SERIAL NOT NULL,
    "organisationId" INTEGER NOT NULL,
    "institutionId" INTEGER NOT NULL,
    "externalId" TEXT NOT NULL,
    "connectUrl" TEXT NOT NULL,

    CONSTRAINT "InstitutionConnection_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "InstitutionConnectionAccount" (
    "id" SERIAL NOT NULL,
    "externalId" TEXT NOT NULL,
    "institutionConnectionId" INTEGER NOT NULL,
    "ownerName" TEXT,
    "iban" TEXT,

    CONSTRAINT "InstitutionConnectionAccount_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE UNIQUE INDEX "User_externalId_key" ON "User"("externalId");

-- CreateIndex
CREATE UNIQUE INDEX "Organisation_slug_key" ON "Organisation"("slug");

-- CreateIndex
CREATE UNIQUE INDEX "Institution_externalId_key" ON "Institution"("externalId");

-- CreateIndex
CREATE UNIQUE INDEX "InstitutionConnection_organisationId_externalId_key" ON "InstitutionConnection"("organisationId", "externalId");

-- CreateIndex
CREATE UNIQUE INDEX "InstitutionConnectionAccount_institutionConnectionId_extern_key" ON "InstitutionConnectionAccount"("institutionConnectionId", "externalId");

-- AddForeignKey
ALTER TABLE "OrganisationUser" ADD CONSTRAINT "OrganisationUser_userId_fkey" FOREIGN KEY ("userId") REFERENCES "User"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "OrganisationUser" ADD CONSTRAINT "OrganisationUser_organisationId_fkey" FOREIGN KEY ("organisationId") REFERENCES "Organisation"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "InstitutionConnection" ADD CONSTRAINT "InstitutionConnection_organisationId_fkey" FOREIGN KEY ("organisationId") REFERENCES "Organisation"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "InstitutionConnection" ADD CONSTRAINT "InstitutionConnection_institutionId_fkey" FOREIGN KEY ("institutionId") REFERENCES "Institution"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "InstitutionConnectionAccount" ADD CONSTRAINT "InstitutionConnectionAccount_institutionConnectionId_fkey" FOREIGN KEY ("institutionConnectionId") REFERENCES "InstitutionConnection"("id") ON DELETE RESTRICT ON UPDATE CASCADE;
