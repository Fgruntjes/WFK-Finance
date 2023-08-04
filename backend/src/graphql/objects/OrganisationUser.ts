import { Field, ObjectType } from "type-graphql";
import { Organisation } from "./Organisation";
import { User } from "./User";
import { UserRole } from "./UserRole";

@ObjectType()
export class OrganisationUser {
  @Field()
  organisation: Organisation;

  @Field()
  user: User;

  @Field()
  role: UserRole;
}
