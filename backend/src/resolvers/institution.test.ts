import { describe, it } from 'mocha';
import { executor } from '@/test/executor';
import { parse } from 'graphql';
import assert from 'assert';

describe('sum module', () => {
  it('adds 1 + 2 to equal 3', async () => {
    const result = await executor({
      document: parse(/* GraphQL */ `
        query {
          greetings
        }
      `)
    });
    assert.equal(result, { greetings: 'Hello World!' });
  });
});
