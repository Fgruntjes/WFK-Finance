import { Field, ID, ObjectType } from "type-graphql";
import { Institution } from "./Institution";
import { Organisation } from "./Organisation";

@ObjectType()
export class InstitutionConnection {
  @Field(_ => ID)
  id: string;

  @Field()
  organisation: Organisation;

  @Field()
  institution: Institution;

  @Field()
  externalId: string;

  @Field()
  accounts: InstitutionConnectionAccount[];
}

export class InstitutionConnectionAccount {
  @Field(_ => ID)
  id: string;

  @Field()
  externalId: string;

  @Field({ nullable: true })
  ownerName?: string;

  @Field({ nullable: true })
  iban?: string;
}
