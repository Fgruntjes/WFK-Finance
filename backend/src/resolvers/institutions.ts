import { Context } from "../context";

export const institutions = (_parent: unknown, _args: unknown, context: Context) => {
    return context.prisma.institution.findMany();
};