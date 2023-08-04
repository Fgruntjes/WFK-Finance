import { Field, ID, ObjectType } from "type-graphql";
import { InstitutionConnection } from "./InstitutionConnection";

@ObjectType()
export class Institution {
  @Field(_ => ID)
  id: string;

  @Field()
  externalId: string;

  @Field()
  name: string;

  @Field({nullable: true})
  logo?: string;

  @Field()
  countries: string[];

  @Field(_ => [InstitutionConnection])
  connections: InstitutionConnection[];
}
