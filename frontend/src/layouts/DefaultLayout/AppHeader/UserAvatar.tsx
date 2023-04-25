import { Avatar, Stack } from '@chakra-ui/react';

type UserAvatarProps = {
    name?: string;
    src?: string;
};

function UserAvatar({ name, src }: UserAvatarProps) {
    return (
        <Stack direction="row" alignItems="center">
            <Avatar name={name} src={src} />
            <span>{name}</span>
        </Stack>
    );
}

export default UserAvatar;