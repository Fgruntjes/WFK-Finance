import { createServer } from 'http'
import { yoga } from '@/bootstrap/yoga'

async function main() {  
  const server = createServer(yoga)
   
  server.listen(4000, () => {
    console.log('Server is running on http://localhost:4000')
  })
}
   
main().catch(error => {
  console.error(error)
  process.exit(1)
})