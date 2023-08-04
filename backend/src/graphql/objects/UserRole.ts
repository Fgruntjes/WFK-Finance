import { registerEnumType } from "type-graphql";

export enum UserRole {
  ADMIN = "ADMIN",
  USER = "USER",
}

registerEnumType(UserRole, {
  name: "UserRole",
  description: "Role for the user within the organisation",
});
