
import { get } from '@/utils/http/axios';
enum URL {
  info = '/custSpace/info'
}

export interface LoginData {
  username: string;
  password: string;
}

const getSpaceInfo = async () => get<any>({ url: URL.info });
export { getSpaceInfo };
