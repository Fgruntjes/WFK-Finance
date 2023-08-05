import { Context } from "../context";

export const institution = (_parent: unknown, args: {id: number}, context: Context) => {
    return context.prisma.institution.findUnique({
        where: {
            id: args.id
        }
    });
};