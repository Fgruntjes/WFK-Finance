import { Field, ID, ObjectType } from "type-graphql";
import { OrganisationUser } from "./OrganisationUser";

@ObjectType()
export class Organisation {
  @Field(_ => ID)
  id: string;

  @Field()
  users: OrganisationUser[];
}
