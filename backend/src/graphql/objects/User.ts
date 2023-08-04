import { Field, ObjectType } from "type-graphql";
import { Organisation } from "./Organisation";
import { OrganisationUser } from "./OrganisationUser";

@ObjectType()
export class User {
  @Field()
  organisation: Organisation;

  @Field()
  externalId: string;

  @Field()
  organisations: OrganisationUser[];
}
